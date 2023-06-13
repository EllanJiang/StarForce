# coding=utf-8
##
# @author: bommy
# @brief: xls data 转二进制 工具
import xlrd
import os
from . import jsontolua
import sys
import hashlib
import importlib
from unityparser import UnityDocument
import base64
from .gen_excel_look_up import *

importlib.reload(sys)
from google.protobuf import json_format
from google.protobuf.descriptor import FieldDescriptor as FD
from .util import *
from .hotfix import gen_hotfix_patch_excel
from .hotfix import write_hotfix_lua
import string

ITEM_START_ROW = 3
FIELD_RULE_ROW = 1
FIELD_TYPE_ROW = 0
language_not_contain_chinese = {}
class DataParser:
    """解析excel的数据"""

    def __init__(self, xls_file_path, root_path):
        self._xls_file_path = xls_file_path
        # 不带扩展名的文件名
        self._xls_file_name = os.path.splitext(os.path.basename(self._xls_file_path))[0]
        self.root_path = root_path
        try:
            self._workbook = xlrd.open_workbook(self._xls_file_path)
        except BaseException as e:
            print("open xls file(%s) failed!" % self._xls_file_path)
            raise
        self._sheet_names = self._workbook.sheet_names()
        lookup_json_file = load_config_file("excel/BattleConfigLookup.json")
        self._toclient_sheets = lookup_json_file["toClient"]
        self._battle_sheets = lookup_json_file["configs"]
        # 标记timeDate字段用于服务器lua表
        self._time_field_map = {}

    def _get_out_path(self, ext):
        if ext == ".py":
            return os.path.join(self.root_path, "python");
        elif ext == ".bytes":
            return os.path.join(self.root_path, "bin");
        elif ext == ".txt":
            return os.path.join(self.root_path, "text");
        elif ext == ".lua":
            return os.path.join(self.root_path, "lua_table_out");
        elif ext == ".json":
            return os.path.join(self.root_path, "language_json_out");
        elif ext == '.asset':
            return os.path.join(self.root_path, "language_asset_out");

    def _gen_file(self):
        global language_not_contain_chinese
        useful_sheet_count = 0
        '''遍历所有的sheet 读取.bytes'''
        for name in self._sheet_names:
            self.languageList = {}
            self.not_contain_chinese_list = {}
            # if not name.encode('UTF-8').isalpha() or not name[0].isupper():
            if contain_chinese(name):
                print("SKIP: file (%s) sheet (%s)" % (self._xls_file_path, name))
                continue

            useful_sheet_count = useful_sheet_count+1
            if useful_sheet_count>1:
                raise BaseException("表格存在多个需要解析的sheet-->"+self._xls_file_name)

            print("gen xls sheet(%s) -> pb bytes" % name)
            try:
                self._sheet = self._workbook.sheet_by_name(name)
            except BaseException as e:
                print("open sheet(%s) failed!" % name)
                continue
            if "GameModeConfig" == self._xls_file_name:
                a = 1

            try:
                sys.path.append(self._get_out_path(".py"))
                # 用xls的文件名命名
                self._module_name = "xls_beans_" + self._xls_file_name.lower() + "_pb2"
                exec('from ' + self._module_name + ' import *')
                self._module = sys.modules[self._module_name]
            except BaseException as e:
                print("load module(%s) failed" % (self._module_name))
                raise

            self._pb_file_name = "xls_beans_" + self._xls_file_name.lower() + ".proto"
            self._row_count = self._sheet.nrows
            self._col_count = self._sheet.ncols
            self._row = 0
            self._col = 0
            self._is_in_struct = False
            self._keyList = []
            try:
                self._gen_bytes()
            except BaseException as e:
                print("Analysis xls describe error excel name:(%s) sheet name:(%s) row:(%d),col:(%d)"
                      % (self._xls_file_path, self._xls_file_name, self._row, self._col - 1))
                raise
        # self._write_asset(language_not_contain_chinese,'no_translation')

    def _gen_bytes(self):
        if self._xls_file_name == "RoomSetConfig":
            print(self._xls_file_name )
        '''对于每一个sheet 读取.bytes'''
        item_array = getattr(self._module, self._xls_file_name + '_Array')()
        # 确定id是否在第0/1列，可能第0列为描述
        key_col = 0
        is_exit_id_col = False
        row_list = []
        for col in range(self._col_count):
            if str(self._sheet.cell_value(1, col)).strip() != "":
                key_col = col
                if str(self._sheet.cell_value(1, col)).strip() == "id":
                    is_exit_id_col = True
                break
        for self._row in range(ITEM_START_ROW, self._row_count):
            # 如果 id 是 空 直接跳过改行
            info_id = str(self._sheet.cell_value(self._row, key_col)).strip()
            if info_id == "":
                continue
            if is_exit_id_col and self._keyList.__contains__(info_id):
                raise BaseException("{} exit repeat key: {}".format(self._xls_file_name, info_id))
            self._keyList.append(info_id)
            item = item_array.items.add()
            self._col = 0
            self._parse_line(item)
            row_list.append(self._row+1)

        to_client = (self._xls_file_name.lower() in self._toclient_sheets)
        # self._write_asset(self.languageList,self._xls_file_name.lower())
        self._write_readable_data_to_file(item_array, to_client)
        if self._xls_file_name.lower() in self._battle_sheets:
            data = item_array.SerializeToString()
            gen_hotfix_patch_excel(self._xls_file_name ,data,row_list ,len(data))
            self._write_data_to_file(data, to_client)

    def _parse_line(self, item ):
        '''每一个item读取一行'''
        # 按照生成的proto顺序，逐列读取数据，插入对应的字段中
        for field in item.DESCRIPTOR.fields:
            field_rule = str(self._sheet.cell_value(FIELD_RULE_ROW, self._col)).strip()
            # 空列处理
            while field_rule == "":
                self._col += 1
                if self._col < self._col_count:
                    field_rule = str(self._sheet.cell_value(FIELD_RULE_ROW, self._col)).strip()
                else:
                    raise BaseException("field_rule is null and _col is beyond max")

            if field.label == FD.LABEL_REPEATED:
                if field.type == FD.TYPE_MESSAGE:
                    while self._col < self._col_count:
                        field_rule_cur = str(self._sheet.cell_value(FIELD_RULE_ROW, self._col)).strip()
                        # 注释列
                        if field_rule_cur == "":
                            self._col += 1
                            continue
                        if field_rule_cur != "" and field_rule_cur.find(field.name) == -1:
                            break
                        if not self._check_cur_struct_has_data():
                            continue
                        struct_item = item.__getattribute__(field.name).add()
                        self._parse_struct(struct_item)
                else:
                    while self._col < self._col_count:
                        field_rule_cur = str(self._sheet.cell_value(FIELD_RULE_ROW, self._col)).strip()
                        # 注释列
                        if field_rule_cur == "":
                            self._col +=1
                            continue
                        if field_rule_cur != "" and field_rule_cur.find(field.name) == -1:
                            break
                        field_value = self._get_field_value(field.type, field)
                        if field_value != None:
                            if isinstance(field_value,  list):
                                for v in field_value:
                                    item.__getattribute__(field.name).append(v)
                            else:
                                item.__getattribute__(field.name).append(field_value)
            else:
                if field.type == FD.TYPE_MESSAGE:
                    mes = item.__getattribute__(field.name)
                    self._parse_struct(mes)
                else:
                    field_value = self._get_field_value(field.type, field)
                    if field_value != None and field_rule.find(field.name) != -1:
                        item.__setattr__(field.name, field_value)

    def _check_cur_struct_has_data(self):
        """判断当前的格子有没有数据"""
        info_id = str(self._sheet.cell_value(self._row, self._col)).strip()
        if info_id == "":
            self._col += 1
            return False
        return True

    def _parse_struct(self, struct_item):
        """嵌套结构数据读取"""
        for field in struct_item.DESCRIPTOR.fields:
            while self._col < self._col_count:
                field_rule_cur = str(self._sheet.cell_value(FIELD_RULE_ROW, self._col)).strip()
                # 注释列
                if field_rule_cur == "":
                    self._col += 1
                    continue
                else:
                    break
            field_value = self._get_field_value(field.type, field)
            if field_value != None:
                struct_item.__setattr__(field.name, field_value)


    def _get_field_value(self, field_type, field):
        global language_not_contain_chinese
        """将pb类型转换为python类型"""
        field_value = self._sheet.cell_value(self._row, self._col)
        ctype = self._sheet.cell(self._row, self._col).ctype
        field_describ = self._sheet.cell_value(FIELD_TYPE_ROW, self._col)

        raw_field_name = self._sheet.cell_value(FIELD_RULE_ROW, self._col)
        self._col += 1
        if field_value == "":
            return None
        if len(str(field_value).strip()) <= 0:
            return None
        try:
            if raw_field_name.find("{}") != -1:
                value = str(field_value)
                values = value.split(",")
                res = []
                if field_type == FD.TYPE_UINT32 or field_type == FD.TYPE_SINT32 or field_type == FD.TYPE_FIXED32 \
                        or field_type == FD.TYPE_INT32 or field_type == FD.TYPE_SFIXED32 or field_type == FD.TYPE_INT64 \
                        or field_type == FD.TYPE_UINT64 or field_type == FD.TYPE_SINT64 or field_type == FD.TYPE_FIXED64 \
                        or field_type == FD.TYPE_SFIXED64:
                    for v in values:
                        res.append(int(v))
                elif field_type == FD.TYPE_DOUBLE or field_type == FD.TYPE_FLOAT:
                    for v in values:
                        res.append(float(v))
                return res

            elif field_type == FD.TYPE_INT64 and field_describ == "DateTime":
                self._time_field_map[field.name] = 1
                return int(time.mktime(time.strptime(str(field_value), "%Y-%m-%d %H:%M:%S")))
            elif field_type == FD.TYPE_UINT32 \
                    or field_type == FD.TYPE_SINT32 \
                    or field_type == FD.TYPE_FIXED32 \
                    or field_type == FD.TYPE_INT32 \
                    or field_type == FD.TYPE_SFIXED32:
                if isinstance(field_value, float):
                    return int(round(field_value))
                else:
                    return int(field_value)
            elif field_type == FD.TYPE_INT64 \
                    or field_type == FD.TYPE_UINT64 \
                    or field_type == FD.TYPE_SINT64 \
                    or field_type == FD.TYPE_FIXED64 \
                    or field_type == FD.TYPE_SFIXED64:
                return int(field_value)
            elif field_type == FD.TYPE_DOUBLE or field_type == FD.TYPE_FLOAT:
                return float(field_value)
            elif field_type == FD.TYPE_STRING:
                if ctype == 2 and field_value % 1 == 0.0:
                    field_value = int(field_value)
                field_value = str(field_value)
                if len(field_value) <= 0:
                    return None
                elif field_describ == "string_pure":
                    return field_value
                else:
                    if not field_value.endswith(".png") and self._col != 1:
                        show_key = field_value
                        strMd5 = get_str_md5_9(show_key)
                        # field_value = field_value.replace(r"\\", "\\").replace(r"\n", "\n").replace(r"\r", "\r")
                        if show_key.startswith("C_") or show_key.startswith("dp_C_"):
                            self.languageList[strMd5] = [field_value, show_key]
                        else:
                            language_not_contain_chinese[strMd5] = [field_value, show_key]
                            self.not_contain_chinese_list[strMd5] = [field_value, show_key]
                        return strMd5
                    else:
                        return field_value
            elif field_type == FD.TYPE_BYTES:
                field_value = str(field_value).encode('utf-8')
                if len(field_value) <= 0:
                    return None
                else:

                    return field_value
            elif field_type == FD.TYPE_BOOL:
                field_value = int(field_value)
                if field_value == 1:
                    return True
                elif field_value == 0:
                    return False
                else:
                    return False
            else:
                return None
        except BaseException as e:
            print("parse cell(%u, %u) error, please check it, maybe type is wrong." % (self._row, self._col))
            raise



    def _write_data_to_file(self, data, to_client):
        '''将数据写入bytes文件中，供unity读取'''
        file_name = "Configs_" + self._xls_file_name.lower() + ".bytes"
        name = os.path.join(self._get_out_path(".bytes"), file_name)
        f = open(name, 'wb+')
        f.write(data)
        f.close()
        if to_client:
            CopyFiles(name, self._get_out_path(".bytes") + "_client")



   
    # def _write_asset(self,language,sheet_name):
    #     try:
    #         # 生成Asset格式语言包
    #         if len(language) > 0:
    #             language_asset_file_name ="tbl_" + sheet_name.lower() + "_language.asset"
    #             language_asset_name = os.path.join(self._get_out_path('.asset'),language_asset_file_name)
    #             src_asset_path = os.path.join(os.getcwd(),'template.asset')
    #             shutil.copyfile(src_asset_path,language_asset_name)
    #             doc = UnityDocument.load_yaml(language_asset_name)
    #             ProjectSettings = doc.entry
    #             ProjectSettings.m_Name = "tbl_" + sheet_name.lower() + "_language"
    #             arrays = []
    #             for(k,v) in language.items():
    #                 dic = {}
    #                 dic['showkey']=v[1]
    #                 dic['key'] = int(k)
    #                 dic['type'] = 0
    #                 value_dic = {}
    #                 sb = bytes(v[0],encoding = 'utf-8')
    #                 if ("<测试>" in v[0]) or ("<test>" in v[0]):
    #                     continue
    #                 str_result = base64.b64encode(sb)
    #                 value_dic['dataValue'] = str(str_result ,encoding = "utf-8")
    #                 dic['value'] = value_dic
    #                 arrays.append(dic)
    #             ProjectSettings.Source['entries'] = arrays
    #             doc.dump_yaml(language_asset_name)
    #     except BaseException as e:
    #         print("write excel item by asset error:(%s)" % self._xls_file_name)
    #         raise

    def _write_readable_data_to_file(self, array, to_client):
        # 转excel 集成的items -> json 列表
        file_name = "Configs_" + self._xls_file_name.lower() + ".txt"
        name = os.path.join(self._get_out_path(".txt"), file_name)
        try:
            request = json_format.MessageToJson(array, True, True, 2, True, False, None, 5,False)
            # None, 5,False)
            with open(name, 'w', encoding='utf-8') as f:
                f.write(request)
            # 在这里生成lua直接使用的table表
            jsontolua.json_to_lua_file(request, self._xls_file_name, self._get_out_path(".lua"), self.languageList, self.not_contain_chinese_list, to_client, self._time_field_map)
            write_hotfix_lua(request,self._xls_file_name)
            # 生成语言包
            languageFile_name = "Configs_" + self._xls_file_name.lower() + "_language.txt"
            language_name = os.path.join(self._get_out_path(".txt"), languageFile_name)
            if len(self.languageList) > 0:
                languageFile_name = "tbl_" + self._xls_file_name.lower() + "_language.json"
                language_name = os.path.join(self._get_out_path(".json"), languageFile_name)
                language_json = json.dumps(self.languageList, ensure_ascii=False, indent=2)

                with open(language_name, 'w', encoding='utf-8') as luafile:
                    luafile.write(language_json)

        except BaseException as e:
            print("write excel item by json error:(%s)" % self._xls_file_name)
            raise

    # md5后9位数字
    def _get_str_md5_9(self,parmStr):
        parmStr = parmStr.encode("utf-8")
        m = hashlib.md5()
        m.update(parmStr)
        result = int(m.hexdigest(), 16)
        result = str(result)[-9:]
        return result

    # md5后4位数字
    def _get_str_md5_4(self, parmStr):
        parmStr = parmStr.encode("utf-8")
        m = hashlib.md5()
        m.update(parmStr)
        result = int(m.hexdigest(), 16)
        result = str(result)[-4:]
        return result

def to_table_name(name):
    l = "tbl"
    for x in name:
        if x.isupper():
            l = l + "_" + x.lower()
        else:
            l = l + x
    return l

def ConvertXls2Data(filepath, out_dir):
    parser = DataParser(filepath, out_dir)
    parser._gen_file()


def main():
    """入口"""
    ROOT_PATH = os.path.split(os.path.realpath(__file__))[0]
    XLS_PATH = ROOT_PATH + r"\xls"
    for dirpath, dirnames, filenames in os.walk(XLS_PATH):
        for filepath in filenames:
            key = filepath
            fullpath = os.path.join(dirpath, filepath)
            fp = os.path.splitext(fullpath)
            if fp[1] == ".xlsx":
                parser = DataParser(fullpath, ROOT_PATH)
                parser._gen_file()

    # for f in os.listdir(XLS_PATH):
    # if os.path.splitext(f)[1] == ".xls":
    # xls_file_path = os.path.join('xls', f)
    # # gen xls - > .bytes
    # parser = DataParser(xls_file_path)
    # parser._gen_file()

if __name__ == "__main__":
    main()
