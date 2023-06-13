# coding=utf-8
from google.protobuf import json_format
import os
import sys
import shutil
import glob
from .util import Mkdirs
from .hotfix import gen_hotfix_patch_json

ROOT_PATH = os.getcwd() + os.sep
BIN_OUT_DIR = ROOT_PATH + 'bin_out' + os.sep
JSON_CONFIG_DIR = ROOT_PATH + 'json_config' + os.sep
PYTHON_OUT_DIR = ROOT_PATH + 'python_out' + os.sep
META_STR = "{} {} {}"

def load_pb_file(py_dir):
    sys.path.append(py_dir)
    for f in os.listdir(py_dir):
        if os.path.splitext(f)[1] == ".py":
            module_name = f[:-3]
            if module_name == "__init__":
                continue
            try:
                exec ('from ' + module_name + ' import *')
            except BaseException as e:
                print("load module(%s) failed" % (module_name))
                raise e

def write_data_to_file(data, out_dir, index):
    classname = "allJson" + index + ".bytes"
    filename = os.path.join(out_dir, classname)
    f = open(filename, 'ab')
    f.write(data)
    # print("gen json beans: " + classname)
    f.close()

def write_meta_data_to_file(output, out_dir, index):
    classname = "allMeta" + index + ".bytes"
    filename = os.path.join(out_dir, classname)
    f = open(filename, 'ab')
    for line in output:
        f.write(line.encode("utf8"))
    print(("gen json beans: " + classname))
    f.close()

def TruncateFile(filepath):
    if os.path.exists(filepath):
        f = open(filepath, 'r+')
        f.truncate()
        f.close()

def clear_old_files(out_dir):
    Mkdirs(out_dir)
    filename1 = os.path.join(out_dir, "allJson.bytes")
    TruncateFile(filename1)    

    filename2 = os.path.join(out_dir, "allMeta.bytes")
    TruncateFile(filename2)


def GenerateJsonConfig(pattern, out_dir, py_dir ,index):
    clear_old_files(out_dir)
    meta_output = []
    total_offset = 0
    is_first = True
    load_pb_file(py_dir)
    for file in glob.glob(pattern):
        namestr = os.path.splitext(os.path.basename(file))[0]
        name = str.split(namestr, '=')[0]
        try:            
            if not (name + '_pb2' in sys.modules):
                raise Exception("{} file is not exit!! please check".format(name + '_pb2'))
            f = open(file, 'r', encoding='UTF-8')
            module = sys.modules[name + '_pb2']
            request = json_format.Parse(f.read(), module.__getattribute__(name)())
            f.close()
            data = request.SerializeToString()
            lenth = len(data)
            write_data_to_file(data, out_dir, index)
            gen_hotfix_patch_json(data,namestr,lenth)
            # 生成meta
            if is_first:
                is_first = False
            else:
                meta_output.append("|")
            meta_output.append(META_STR.format(namestr, total_offset, lenth))
            total_offset = total_offset + lenth
        except BaseException as e:
            print("file json (%s) config failed" % (file))
            raise e
    write_meta_data_to_file(meta_output, out_dir, index)


def main():
    meta_output = []
    total_offset = 0
    is_first = True
    clear_old_files("bin_out")
    load_pb_file(PYTHON_OUT_DIR)
    for indexs in range(1, 4):
        stri = str(indexs)
        strindexs = '_' + str(indexs)
        if strindexs == '_1':
            strindexs = ''
        for file in glob.glob(os.path.join(JSON_CONFIG_DIR + strindex, "*.json")):
            namestr = os.path.splitext(file)[0]
            name = str.split(namestr, '=')[0]
            try:
                path_name = os.path.join('json_config' + strindex, file)
                if not (name + '_pb2' in sys.modules):
                    raise Exception("gen_json/python_out/{} file is not exit!! please check".format(name + '_pb2'))
                f = open(path_name, 'r')
                module = sys.modules[name + '_pb2']
                request = json_format.Parse(f.read(), module.__getattribute__(name)())
                f.close()
                data = request.SerializeToString()
                length = len(data)
                write_data_to_file(data, "out/json/bin", stri)
                # 生成meta
                if is_first:
                    is_first = False
                else:
                    meta_output.append("|")
                meta_output.append(META_STR.format(namestr, total_offset, length))
                total_offset = total_offset + length
            except BaseException as e:
                print("open json (%s) failed" % (file))
                raise
        write_meta_data_to_file(meta_output, "bin_out", stri)

if __name__ == "__main__":
    main()
