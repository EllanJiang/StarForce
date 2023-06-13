import json
import os
import hashlib
from .jsontolua import to_table_name
from .jsontolua import convert_list_to_dict
from .jsontolua import dic_to_lua_str

def gen_hotfix_patch_json(data,name ,length):
    content = name + "|"
    # 1表示json 2 表示excel
    content = content + "1" + "|"
    content = content + "0" + "|"
    content = content + str(length) + ";"
    config_path = "out/hotfix_config.bytes"
    with open(config_path , 'a+') as file:
       file.write(content)

    data_path = "out/hotfix_data.bytes"
    with open(data_path , 'ab') as file:
        file.write(data)


def gen_hotfix_patch_excel(sheet_name,data,row_list ,length):
    content = str.capitalize(sheet_name) + "|"
    # 1表示json 2 表示excel
    content = content + "2" + "|"
    list_len = len(row_list)
    list_index = 1
    for i in row_list:
        if (list_index < list_len):
            content = content + str(i) + ","
        else:
            content = content + str(i) + "|"
        list_index = list_index + 1
    content = content + str(length) + ";"
    config_path = "out/hotfix_config.bytes"
    with open(config_path , 'a+') as file:
       file.write(content)

    data_path = "out/hotfix_data.bytes"
    with open(data_path , 'ab') as file:
        file.write(data)

def json_to_lua(jsonStr,tableName):
    try:
        data_dic = json.loads(jsonStr)
    except Exception as e:
        data_dic = []
    else:
        pass
    finally:
        pass
    data_converted = convert_list_to_dict(data_dic)
    bytes = 'local '
    bytes += tableName
    bytes += ' = '
    for it in dic_to_lua_str(data_converted):
        bytes += it
    bytes += '\n'
    return bytes

def write_hotfix_lua_prefix():
    file_path = "out/hotfix_lua.bytes"
    with open(file_path, 'a+', encoding='utf-8') as luafile:
        content = "global_hotfix_runtime_list = {} \n"
        luafile.write(content)

def write_hotfix_lua_suffix():
    file_path = "out/hotfix_lua.bytes"
    with open(file_path, 'a+', encoding='utf-8') as luafile:
        # content = "HotfixRuntime.Replace(list) \n"
        content = "\n"
        luafile.write(content)

# def convert_hotfix_lua():
#     file_path = "out/hotfix_lua.bytes"
#     luafile = open(file_path, 'r', encoding='utf-8')
#     content = luafile.read()
#     # b = content.encode('utf-8')
#     b = bytes(content ,'utf-8')
#     luafile.close()
#     lua = open("out/hotfix_lua2.bytes",'ab')
#     lua.write(b)
#     lua.close()

def write_hotfix_lua(jsonData, name):
    table_name = to_table_name(name)
    file_path = "out/hotfix_lua.bytes"
    with open(file_path, 'a+', encoding='utf-8') as luafile:
        content = json_to_lua(jsonData,table_name)
        temp = "global_hotfix_runtime_list[" + '\"' + table_name +'\"' +"] =" + str(table_name)
        content += temp
        content += '\n'
        luafile.write(content)

def _rename(path ,suffix):
    file_path = path + suffix
    m = hashlib.md5()
    with open(file_path, 'rb') as file:
        content = file.read()
        m.update(content)
    dst_name = path+m.hexdigest()+suffix
    os.rename(file_path, dst_name)

def rename_hotfix_file_name():
    print("")
    # _rename("out/hotfix_config" ,".bytes")
    # _rename("out/hotfix_data", ".bytes")
    # _rename("out/hotfix_lua", ".bytes")
