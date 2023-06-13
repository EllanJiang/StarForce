# coding=utf-8
##
# @author: bommy
# @brief:  xls 转 proto工具
import xlrd
import sys
import os
from .util import *

# path
ROOT_PATH = os.path.split(os.path.realpath(__file__))[0]

XLS_PATH = ROOT_PATH + r"\xls"
Table_PATH = ROOT_PATH + r"\tabledecoder_out"

# string format
SYNTAX_HEAD = "syntax = \"proto3\";\n\n"
NAMESPACE = "package Configs;\n"
NORMAL_FIELD_STRING = "\t{} {} = {};\n"
REPEATED_FIELD_STRING = "\trepeated {} {} = {};\n"
MESSAGE_HEAD_LINE = "\nmessage {}{{\n"
MESSAGE_TAIL_LINE = "}\n"
COMMENT_LINE = "\t// {} \n"

# gen .cs tabledecoder
USING_STR = "using System;\nusing System.Collections.Generic;\nnamespace Configs\n{\n"
START_CLASS_STR = "\tpublic class {} : BaseTable\n"
# PATH_STR = "\t\tprivate string PATH = \"XlsConfig/xls_beans_{}\";\n"
TABLE_DEFINE_STR = "\t\tprivate Dictionary<{}, {}> {} = new Dictionary<{}, {}>();\n\n"
# format for array save method
TABLE_DEFINE_ARRAY_STR = "\t\tprivate {} {} = new {}();\n\n"
FUNCTION_LOAD_STR = "\t\tpublic override void LoadConfig(byte[] buffer, int offset, int length)\n"
ZERO_TAB_END_STR = "}"
ONT_TAB_START_STR = "\t{\n"
ONT_TAB_END_STR = "\t}\n"
DOUBLE_TAB_START_STR = "\t\t{\n"
DOUBLE_TAB_END_STR = "\t\t}\n"
TRIPLE_TAB_START_STR = "\t\t\t{\n"
TRIPLE_TAB_END_STR = "\t\t\t}\n"
TRY_STR = "\t\t\ttry\n"
# FILE_EXIST_STR = "\t\t\t\tvar obj= Resources.Load<TextAsset>(PATH);\n"
# READ_STR = "\t\t\t\tvar buffer = obj.bytes;\n"
PARSE_STR = "\t\t\t\t{} {} = {}.Parser.ParseFrom(buffer, offset, length);\n"
PARSE_ARRAY_STR = "\t\t\t\t{} = {}.Parser.ParseFrom(buffer, offset, length);\n"
MAP_LOAD_STR = "\t\t\t\tforeach (var item in {}.Items)\n\t\t\t\t{{\n\t\t\t\t\t{}.Add(item.{}, item);\n\t\t\t\t}}\n"
CATCH_STR = "\t\t\tcatch (Exception ex)\n"
ERROR_STR = "\t\t\t\tstring errorMsg = \"{}.LoadConfig Error\\n{{0}}\";\n"
DEBUG_STR = "\t\t\t\tthrow new Exception(string.Format(errorMsg, ex.ToString()));\n"
GET_TABLE_STR = "\t\tpublic Dictionary<{}, {}> GetTable()\n\t\t{{\n\t\t\treturn {};\n\t\t}}\n\n"
GET_TABLE_ARRAY_STR = "\t\tpublic {} GetTable()\n\t\t{{\n\t\t\treturn {};\n\t\t}}\n\n"
GET_RECORDER_STR = "\t\tpublic {} GetRecorder({} key)\n\t\t{{\n\t\t\tif (!{}.ContainsKey(key))\n\t\t\t{{\n\t\t\t\treturn null;\n\t\t\t}}\n\t\t\treturn {}[key];\n\t\t}}\n\n"
GET_RECORDER_ARRAY_STR = "\t\tpublic {} GetRecorder(int key)\n\t\t{{\n\t\t\tif (key >= {}.Items.Count)\n\t\t\t{{\n\t\t\t\treturn null;\n\t\t\t}}\n\t\t\treturn {}.Items[key];\n\t\t}}\n\n"

# TAP的空格数
TAP_BLANK_NUM = 4
# 类型 在第0行， 规则在第一行，注释在第2行
FIELD_TYPE_ROW = 0
FIELD_RULE_ROW = 1
FIELD_COMMENT_ROW = 2


# XLS -> .proto 解析类
class ProtoPaser:
    """通过excel配置生成配置的protobuf定义文件"""

    def __init__(self, xls_file_path, out_path, tabledecoder_dir):
        self.out_path = out_path
        self.tabledecoder_dir = tabledecoder_dir
        self._xls_file_path = xls_file_path
        self._xls_file_name = os.path.splitext(os.path.basename(self._xls_file_path))[0]
        try:
            self._workbook = xlrd.open_workbook(self._xls_file_path)
        except BaseException as e:
            print("open xls file(%s) failed!" % self._xls_file_path)
            raise

        self._sheet_names = self._workbook.sheet_names()
        # 将所有的输出先写到一个list， 最后统一写到文件
        self._output = []
        # 排版缩进空格数
        self._indentation = 0
        self._battle_sheets = load_config_file("excel/BattleConfigLookup.json")["configs"]

    def _gen_file(self):
        useful_sheet_count = 0
        '''遍历所有的sheet 创建.proto arry'''
        for name in self._sheet_names:
            # 整个.proto文件的输出保存
            self._output = []
            # 特殊格式struct的proto定义
            self._output_addition = []
            # []重复字段，需要保存定义的信息，防止重复增加,因为每一列都会有相同的定义
            self._repeated_field_names = []
            # 防止重名，增加符号表
            self._sign_table = []
            # 保存key的类型名
            self._key_type_name = ""
            self._key_name = ""

            if contain_chinese(name):
                print("sheet_name is invalid : " + name)
                continue
            useful_sheet_count = useful_sheet_count + 1
            if useful_sheet_count > 1:
                raise BaseException("表格存在多个需要解析的sheet-->" + self._xls_file_name)
            try:
                self._sheet = self._workbook.sheet_by_name(name)
            except BaseException as e:
                print("open sheet(%s) failed!" % name)
                continue
            self._pb_file_name = "xls_beans_" + self._xls_file_name.lower() + ".proto"
            self._col_count = self._sheet.ncols
            self._col = 0
            self._index = 1
            self._index_addition = 1
            self._is_in_struct = False
            self._addition_message_name = ""
            try:
                self._layout_file()
            except BaseException as e:
                print("Analysis xls describe error sheet name:(%s)" % self._xls_file_name)
                raise

    def _layout_file(self):
        '''对于每一个sheet，生成.proto'''
        self._layout_file_header()
        self._layout_message_head(self._xls_file_name)
        if self._xls_file_name == "RoomSetConfig":
            print("---")
        while self._col < self._col_count:
            self._parse_field_define()
            self._col += 1

        self._layout_message_tail()

        # 增加额外的结构定义
        self._output.extend(self._output_addition)
        # 增加对于数组的定义
        self._layout_array()
        # 写数据到文件
        self._write_to_file()
        # 增加生成table coder文件
        if self._xls_file_name.lower() in self._battle_sheets:
            self._gen_cs_file()

    def _parse_field_define(self):
        field_type = str(self._sheet.cell_value(FIELD_TYPE_ROW, self._col))
        describe = str(self._sheet.cell_value(FIELD_RULE_ROW, self._col))
        if len(describe) == 0 or len(field_type) == 0:
            return
        # 特殊处理时间类型 -> int64
        if field_type == "DateTime":
            field_type = "int64"
        elif field_type == "string_pure":
            field_type = "string"

        if self._key_type_name == "":
            self._key_type_name = field_type
            self._key_name = describe
        if self._check_has_field(describe):
            return

        if describe.find("{}") != -1:
            "支持逗号分隔的数组，仅支持最简单的形式，不支持结构体数组"
            self._repeated_field_names.append(describe)
            field_name = str.split(describe, "{}")
            if describe.find(".") != -1:
                raise BaseException("{}'s field {} is a simple array. Structure not supported".format(self._xls_file_name, describe))
            self._add_line(True, field_type, field_name[0], self._index, describe)
            self._index += 1
        elif describe.find("[]") != -1:
            '''数组'''
            self._repeated_field_names.append(describe)
            names1 = str.split(describe, "[]")
            if describe.find(".") != -1:
                '''结构体'''
                names = str.split(names1[1], ".")
                self._addition_message_name = names1[0]
                if not self._is_in_struct:
                    self._add_line(True, self._xls_file_name + "_" + names1[0], names1[0], self._index, describe)
                    self._index += 1
                    self._index_addition = 1
                    # 增加message开头
                    self._output_addition.append(MESSAGE_HEAD_LINE.format(self._xls_file_name + "_" + names1[0]))
                    self._is_in_struct = True

                self._add_line_addition(False, field_type, names[1], self._index_addition)
                self._index_addition += 1
                self._check_next()
            else:
                self._add_line(True, field_type, names1[0], self._index, describe)
                self._index += 1
        else:
            '''非数组'''
            if describe.find(".") != -1:
                '''结构体'''
                names = str.split(describe, ".")
                self._addition_message_name = names[0]
                if not self._is_in_struct:
                    self._add_line(False, self._xls_file_name + "_" + names[0], names[0], self._index, describe)
                    self._index += 1
                    self._index_addition = 1
                    # 增加message开头
                    self._output_addition.append(MESSAGE_HEAD_LINE.format(self._xls_file_name + "_" + names[0]))
                    self._is_in_struct = True

                self._add_line_addition(False, field_type, names[1], self._index_addition)
                self._index_addition += 1
                self._check_next()

            else:
                self._add_line(False, field_type, describe, self._index, describe)
                self._index += 1

    def _check_has_field(self, field_name):
        """如果是重复字段，需先进入这里，判断是够字段是够已经存在"""
        for name in self._repeated_field_names:
            if name == field_name:
                self._check_next()
                return True
        return False

    def _check_has_name(self, field_name):
        """如果是item的字段，需先进入这里，判断是够字段是够已经存在"""
        for name in self._sign_table:
            if name == field_name:
                return True
        return False

    def _layout_file_header(self):
        """生成PB文件的描述信息"""
        self._output.append("/**\n")
        self._output.append("* @file:   " + self._xls_file_path + "\\" + self._xls_file_name + "\n")
        self._output.append("* @author: lth\n")
        self._output.append("* @brief:  这个文件是通过工具自动生成的，不要手动修改!\n")
        self._output.append("*/\n\n")
        self._output.append(SYNTAX_HEAD)
        self._output.append(NAMESPACE)

    def _layout_message_head(self, message_name):
        """生成结构头"""
        self._output.append(MESSAGE_HEAD_LINE.format(message_name))

    def _layout_message_tail(self):
        """生成结构尾"""
        self._output.append(MESSAGE_TAIL_LINE)

    def _add_line(self, is_repeated, field_type, field_name, index, describe):
        """输出文件数组增加一行定义"""
        if self._check_has_name(describe):
            raise BaseException("redefine field name: %s" % describe)
        else:
            self._sign_table.append(describe)

        field_comment = str(self._sheet.cell_value(FIELD_COMMENT_ROW, self._col)).replace('\n', '')
        self._output.append(COMMENT_LINE.format(field_comment))

        if is_repeated:
            self._output.append(REPEATED_FIELD_STRING.format(field_type, field_name, index))
        else:
            self._output.append(NORMAL_FIELD_STRING.format(field_type, field_name, index))

    def _add_line_addition(self, is_repeated, field_type, field_name, index):
        """结构体增加定义"""
        field_comment = str(self._sheet.cell_value(FIELD_COMMENT_ROW, self._col)).replace('\n', '')
        self._output_addition.append(COMMENT_LINE.format(field_comment))
        if is_repeated:
            self._output_addition.append(REPEATED_FIELD_STRING.format(field_type, field_name, index))
        else:
            self._output_addition.append(NORMAL_FIELD_STRING.format(field_type, field_name, index))

    def _check_next(self):
        """检测 struct 是否读取完成"""
        if self._addition_message_name == "":
            return
        temp_col = self._col+1
        while temp_col < self._col_count:
            next_describe = str(self._sheet.cell_value(FIELD_RULE_ROW, temp_col))
            temp_col +=1
            if next_describe == "":
                continue
            if next_describe != "" and next_describe.find(self._addition_message_name) != -1:
                return

        self._is_in_struct = False
        self._output_addition.append(MESSAGE_TAIL_LINE)
        self._addition_message_name = ""
        self._index_addition = 1
        self._repeated_field_names = []

    def _layout_array(self):
        """输出数组定义"""
        self._output.append("\nmessage " + self._xls_file_name + "_Array {\n")
        self._output.append("\trepeated " + self._xls_file_name + " items = 1;\n}\n")

    def _write_to_file(self):
        """输出.proto到文件"""
        name = os.path.join(self.out_path, self._pb_file_name)
        pb_file = open(name, "w+", encoding="utf-8")
        pb_file.writelines(self._output)
        pb_file.close()
        print("gen xls proto Success : " + self._pb_file_name)

    def _gen_cs_file(self):
        """ gen .cs tabledecoder """
        class_name = self._xls_file_name.capitalize() + "Table"
        single_item_name = self._xls_file_name
        arry_name = single_item_name + "_Array"
        bin_name = self._xls_file_name.lower()
        arry_var_name = arry_name.lower()
        map_var_name = single_item_name.lower() + "_map"
        key_type_name = self._proto_type_to_csharp_type(self._key_type_name)
        is_array = (self._key_name != "id")
        # 代码生成
        self._cs_output = []
        self._cs_output.append(USING_STR)
        self._cs_output.append(START_CLASS_STR.format(class_name))
        self._cs_output.append(ONT_TAB_START_STR)
        #        self._cs_output.append(PATH_STR.format(bin_name))
        if is_array:
            self._cs_output.append(TABLE_DEFINE_ARRAY_STR.format(arry_name, arry_var_name, arry_name))
        else:
            self._cs_output.append(
                TABLE_DEFINE_STR.format(key_type_name, single_item_name, map_var_name, key_type_name, single_item_name))
        self._cs_output.append(FUNCTION_LOAD_STR)
        self._cs_output.append(DOUBLE_TAB_START_STR)
        self._cs_output.append(TRY_STR)
        self._cs_output.append(TRIPLE_TAB_START_STR)
        #        self._cs_output.append(FILE_EXIST_STR.format(bin_name))
        #        self._cs_output.append(READ_STR)
        if is_array:
            self._cs_output.append(PARSE_ARRAY_STR.format(arry_var_name, arry_name))
        else:
            self._cs_output.append(PARSE_STR.format(arry_name, arry_var_name, arry_name))
            self._cs_output.append(MAP_LOAD_STR.format(arry_var_name, map_var_name, self._key_name.capitalize()))
        self._cs_output.append(TRIPLE_TAB_END_STR)
        self._cs_output.append(CATCH_STR)
        self._cs_output.append(TRIPLE_TAB_START_STR)
        self._cs_output.append(ERROR_STR.format(arry_name))
        self._cs_output.append(DEBUG_STR)
        self._cs_output.append(TRIPLE_TAB_END_STR)
        self._cs_output.append(DOUBLE_TAB_END_STR)
        self._cs_output.append("\n")
        if is_array:
            self._cs_output.append(GET_TABLE_ARRAY_STR.format(arry_name, arry_var_name))
            self._cs_output.append(GET_RECORDER_ARRAY_STR.format(single_item_name, arry_var_name, arry_var_name))
        else:
            self._cs_output.append(GET_TABLE_STR.format(key_type_name, single_item_name, map_var_name))
            self._cs_output.append(GET_RECORDER_STR.format(single_item_name, key_type_name, map_var_name, map_var_name))
        self._cs_output.append(ONT_TAB_END_STR)
        self._cs_output.append(ZERO_TAB_END_STR)

        name = os.path.join(self.tabledecoder_dir, class_name + ".cs")
        pb_file = open(name, "w+")
        pb_file.writelines(self._cs_output)
        pb_file.close()
        print("gen xls table coder Success  : " + class_name)

    def _proto_type_to_csharp_type(self, proto_name):
        '''protobuff 类型转 unity c# 类型，不支持类型：浮点，bytes'''
        if proto_name == "int32" or proto_name == "sint32" or proto_name == "sfixed32":
            return "int"
        elif proto_name == "int64" or proto_name == "sint64" or proto_name == "sfixed64":
            return "long"
        elif proto_name == "uint32":
            return "uint"
        elif proto_name == "uint64" or proto_name == "fixed64":
            return "ulong"
        elif proto_name == "string" or proto_name == "string_pure":
            return "string"
        else:
            raise BaseException("Type not supported temporarily : %s" % proto_name)


def ConvertXls2Proto(filepath, outdir, tabledecoder_out):
    parser = ProtoPaser(filepath, outdir, tabledecoder_out)
    parser._gen_file()


ROOT_PATH = os.path.split(os.path.realpath(__file__))[0]
XLS_PATH = ROOT_PATH + r"\xls"
PROTO_PATH = os.path.join(ROOT_PATH, "proto_out")
Table_PATH = ROOT_PATH + r"\tabledecoder_out"


def main():
    for dirpath, dirnames, filenames in os.walk(XLS_PATH):
        for filepath in filenames:
            key = filepath
            fullpath = os.path.join(dirpath, filepath)
            fp = os.path.splitext(fullpath)
            if fp[1] == ".xlsx":
                parser = ProtoPaser(fullpath, PROTO_PATH)
                parser._gen_file()


# for f in os.listdir(XLS_PATH):
# if os.path.splitext(f)[1] == ".xls":
# xls_file_path = os.path.join('xls', f)
# # gen xls - > .proto
# parser = ProtoPaser(xls_file_path)
# parser._gen_file()


if __name__ == "__main__":
    main()
