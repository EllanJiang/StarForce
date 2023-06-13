
import json
import types
import json
import os

import sys
import importlib
from .util import *
importlib.reload(sys)
#sys.setdefaultencoding('utf-8')
tableLayerType = 1
languageList = {}
not_contain_chinese_list = {}
def space_str(layer):
    spaces = ""
    for i in range(0,layer):
        spaces += '\t'
    return spaces

def dic_to_lua_str(data,layer=0):
    global tableLayerType
    global languageList
    global row_number
    d_type = type(data)
    if  d_type is (str,) or d_type is str or d_type is str:
        # print(data, )
        data = data.replace("'", "\\'")
        # print(" --to-->", data)
        if (data in languageList) or (data in not_contain_chinese_list):
            yield (str(data))
        else:
            yield ("'" + data + "'")
    elif d_type is bool:
        if data:
            yield ('true')
        else:
            yield ('false')
    elif d_type is int or d_type is int or d_type is float:
        yield (str(data))
    elif d_type is list:
        yield ("{")
        if layer == tableLayerType:
            yield("\n")
            yield (space_str(layer+1))
        for i in range(0,len(data)):
            for sub in  dic_to_lua_str(data[i],layer+1):
                yield sub
            if i < len(data)-1:
                yield (', ')
        if layer == 0:
            yield ('\n')
            yield (space_str(layer))
        yield ('}')
    elif d_type is dict:
        if layer == tableLayerType:
            yield ("\n")
            yield (space_str(layer))
        yield ("{")
        if layer == 0:
            yield("\n")
        elif tableLayerType == 1 and layer == 1:
            yield ('lineNumber = ')
            yield (str(row_number))
            yield (", ")
            row_number = row_number + 1
        data_len = len(data)
        data_count = 0
        for k,v in list(data.items()):
            data_count += 1
            if layer == 0:
                yield (space_str(layer+1))
            if type(k) is int:
                yield ('[' + str(k) + ']')
            else:
                yield (k) 
            yield (' = ')
            try:
                for sub in  dic_to_lua_str(v,layer +1):
                    yield sub

                if data_count < data_len:
                    yield (', ')
                    if layer == 0:
                        yield("\n")

            except Exception as e:
                print('error in ',k,v)
                raise
        if layer == 0:
            yield("\n")
            yield (space_str(layer))
        yield ('}')
    else:
        raise d_type('is error')

def convert_list_to_dict(pylist):
    retData = {}
    global tableLayerType
    global row_number
    tableLayerType = 1
    row_number = 1
    for key in pylist["items"]:
        if "id" not in key:
            return pylist["items"]
        d_type = type(key["id"])
        if d_type is str or d_type is bytes:
            retData["[\"" + key["id"] + "\"]"] = key
        else:
            retData[key["id"]] = key
    tableLayerType = 0
    return retData

def str_to_lua_table(jsonStr,tableName,outDir):
    data_dic = None
    try:
        data_dic = json.loads(jsonStr)
    except Exception as e:
        data_dic =[]
    else:
        pass
    finally:
        pass
    data_converted = convert_list_to_dict(data_dic)
    bytes = ""

    if  outDir:
        bytes = 'local '
        bytes += tableName
        bytes += 'Table = '
        for it in dic_to_lua_str(data_converted):       
            bytes += it
        bytes += '\n'
        bytes += 'return '
        bytes += tableName
        bytes += 'Table'
    else:
        bytes = '_G[\'Tbl'
        bytes += tableName 
        bytes += '\'] = '
        for it in dic_to_lua_str(data_converted):       
            bytes += it
    return bytes

def add_time_field(time_field_map, file_name):
    time_field_str = "\n "+ file_name + " = { \n"
    for field in time_field_map:
        field_str =  "[\"" + field + "\"]" + " = 1,\n"
        time_field_str += field_str
    time_field_str += " \n}"
    return time_field_str

def to_table_name(name):
    l = "tbl"
    name_field = name.split("_", 1)
    for x in name_field[0]:
        if x.isupper():
            l = l + "_" + x.lower()
        else:
            l = l + x
    if len(name_field) >= 2:
        l = l + "_" + name_field[1]
    return l

def get_file_title(name):
    _output = []
    _output.append("--[[\n")
    _output.append("* @file:   " + name + "\n")
    _output.append("* @brief:  这个文件是通过工具自动生成的，不要手动修改!\n")
    _output.append("]]--\n\n")
    return _output


def json_to_lua_file(jsonData, name, outDir, language_list, not_chinese_list, to_client, time_field_map):
    global languageList
    global not_contain_chinese_list
    languageList = language_list
    not_contain_chinese_list = not_chinese_list
    fileName = os.path.join(outDir, to_table_name(name) + ".lua")
    with open(fileName, 'w', encoding='utf-8') as luafile:
        luafile.writelines(get_file_title(name))
        luafile.write(str_to_lua_table(jsonData, name, outDir))

    #生成tbl_field_name用于服务器转时区
    field_table_name = os.path.join(outDir, "tbl_field_name.lua")
    flag_title = False
    if not os.path.exists(field_table_name):
        flag_title = True
    with open(field_table_name, 'a', encoding='utf-8') as time_field_file:
        if flag_title:
            time_field_file.writelines(get_file_title("TblFieldName"))
        if len(time_field_map) != 0:
            time_field_file.write(add_time_field(time_field_map, to_table_name(name)))

    if to_client:
        CopyFiles(fileName, outDir + "_client")


