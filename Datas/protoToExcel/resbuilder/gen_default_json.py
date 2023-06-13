# coding=utf-8
import os
import sys
import shutil
from google.protobuf.descriptor import FieldDescriptor as FD
from google.protobuf import json_format


ROOT_PATH = os.getcwd() + os.sep
PYTHON_OUT_DIR = ROOT_PATH + 'python_out' + os.sep
JSON_DEFAULT_OUT = ROOT_PATH + 'json_default_out' + os.sep
TABLE_DECODER_OUT = ROOT_PATH + 'tabledecoder_out' + os.sep
PROTO_DIR = ROOT_PATH + 'proto' + os.sep

def write_default_data_to_file(data, class_name, out_path):
    name = class_name + ".json"
    filename = os.path.join(out_path, name)
    f = open(filename, 'w')
    f.write(data)
    f.close()


def gen_default_json(cls):
    obj = cls()
    for field in obj.DESCRIPTOR.fields:
        if field.type == FD.TYPE_MESSAGE:
            value = gen_default_json(field.message_type._concrete_class)
            if field.label == FD.LABEL_REPEATED:
                element = getattr(obj, field.name).add()
                element.CopyFrom(value)
            else:
                getattr(obj, field.name).CopyFrom(value)

        elif field.type == FD.TYPE_BOOL:
            setattr(obj, field.name, False)
        elif field.type == FD.TYPE_STRING:
            if field.label == FD.LABEL_REPEATED:
                continue
            setattr(obj, field.name, "")
        elif field.label == FD.LABEL_REPEATED:
            continue
        else:
            setattr(obj, field.name, 0)
    return obj


# -------------gen data reader-----------------------
# ----------------------------------------------------
# gen .cs file string
USING_STR = "using System;\n\nnamespace Configs\n{\n"
START_CLASS_STR = "\tpublic class {}Table : BaseTable\n"
TABLE_DEFINE_STR = "\t\tprivate {} {};\n\n"
FUNCTION_LOAD_STR = "\t\tpublic override void LoadConfig(byte[] buffer, int offset, int length)\n"
ZERO_TAB_END_STR = "}"
ONT_TAB_START_STR = "\t{\n"
ONT_TAB_END_STR = "\t}\n"
DOUBLE_TAB_START_STR = "\t\t{\n"
DOUBLE_TAB_END_STR = "\t\t}\n"
TRIPLE_TAB_START_STR = "\t\t\t{\n"
TRIPLE_TAB_END_STR = "\t\t\t}\n"
TRY_STR = "\t\t\ttry\n"
PARSE_STR = "\t\t\t\t{} = {}.Parser.ParseFrom(buffer, offset, length);\n"
CATCH_STR = "\t\t\tcatch (Exception ex)\n"
ERROR_STR = "\t\t\t\tstring errorMsg = \"{}.LoadConfig Error\\n{{0}}\";\n"
DEBUG_STR = "\t\t\t\tthrow new Exception(string.Format(errorMsg, ex.ToString()));\n"
GET_TABLE_STR = "\t\tpublic {} GetTable()\n\t\t{{\n\t\t\treturn {};\n\t\t}}\n"

def _gen_cs_file(name, out_path):
    """.cs文件编写"""
    class_name = name
    var_name = name.lower()
    # 代码生成
    output = []
    output.append(USING_STR)
    output.append(START_CLASS_STR.format(class_name))
    output.append(ONT_TAB_START_STR)
    output.append(TABLE_DEFINE_STR.format(class_name, var_name))
    output.append(FUNCTION_LOAD_STR)
    output.append(DOUBLE_TAB_START_STR)
    output.append(TRY_STR)
    output.append(TRIPLE_TAB_START_STR)
    output.append(PARSE_STR.format(var_name, class_name))
    output.append(TRIPLE_TAB_END_STR)
    output.append(CATCH_STR)
    output.append(TRIPLE_TAB_START_STR)
    output.append(ERROR_STR.format(class_name))
    output.append(DEBUG_STR)
    output.append(TRIPLE_TAB_END_STR)
    output.append(DOUBLE_TAB_END_STR)
    output.append("\n")
    output.append(GET_TABLE_STR.format(class_name, var_name))
    output.append(ONT_TAB_END_STR)
    output.append(ZERO_TAB_END_STR)
    name = os.path.join(out_path, class_name + "Table.cs")
    pb_file = open(name, "w+")
    pb_file.writelines(output)
    pb_file.close()

def GenDefaultJson(filepath, out_path):
    py_dir = os.path.dirname(filepath)
    sys.path.append(py_dir)

    print(("parse python proto :" + filepath))
    module_name = os.path.splitext(os.path.basename(filepath))[0]
    class_name = module_name[:-4]
    module = None
    try:
        exec ('from ' + module_name + ' import *')
        module = sys.modules[module_name]
    except BaseException as e:
        print("load module(%s) failed" % (module_name))
        raise
    if module_name == "PbVector_pb2":
        return
    obj = gen_default_json(module.__getattribute__(class_name))
    json_str = json_format.MessageToJson(obj, True, True, 2, True)
    # print(json_str)
    write_default_data_to_file(json_str, class_name, out_path)

def GenJsonTableDecoder(filepath, out_dir):
    str_arr = os.path.splitext(os.path.basename(filepath))
    if str_arr[1] == ".proto" and str_arr[0] != "PbVector":
        _gen_cs_file(str_arr[0], out_dir)

def main():
    # 删除文件
    #shutil.rmtree(JSON_DEFAULT_OUT, True)
    if not os.path.exists(JSON_DEFAULT_OUT):
        os.makedirs(JSON_DEFAULT_OUT)
    # data reader code gen
    # 删除文件
    #shutil.rmtree(TABLE_DECODER_OUT, True)
    if not os.path.exists(TABLE_DECODER_OUT):
        os.makedirs(TABLE_DECODER_OUT)

    # iterator .proto -> default json
    sys.path.append(PYTHON_OUT_DIR)
    for file in os.listdir(PYTHON_OUT_DIR):
        if os.path.splitext(file)[1] == ".py":
            print(("parse python proto :" + file))
            module_name = os.path.splitext(file)[0]
            class_name = module_name[:-4]
            module = None
            try:
                exec ('from ' + module_name + ' import *')
                module = sys.modules[module_name]
            except BaseException as e:
                print("load module(%s) failed" % (module_name))
                raise
            if module_name == "PbVector_pb2":
                continue
            obj = gen_default_json(module.__getattribute__(class_name))
            json_str = json_format.MessageToJson(obj, True, True, 2, True)
            # print(json_str)
            write_default_data_to_file(json_str, class_name, "json_default_out")

    for file in os.listdir(PROTO_DIR):
        str_arr = os.path.splitext(file)
        if str_arr[1] == ".proto" and str_arr[0] != "PbVector":
            _gen_cs_file(str_arr[0], "tabledecoder_out")

if __name__ == "__main__":
    main()