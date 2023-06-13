#!/usr/bin/python

import os
import sys

import resbuilder as resBuilder
from resbuilder.util import *
from resbuilder.hotfix import rename_hotfix_file_name
from resbuilder.custom_process import custom_run

PROJECT_PATH = ""
exists_project_path = True
CUSTOM_RUN = False

ExcelSerializePath = ""
ExcelDecoderPath = ""

JsonSerializePath = ""
JsonDecoderPath = ""

ProtoPath = ""

ExcelConfigsPath = ""
JsonlConfigsPath = ""
LuaConfigsPath = ""

def process_xsl():
    # delete old
    delete_file(LuaConfigsPath, 0, "*.lua")
    delete_file(ExcelConfigsPath, 1, "*.bytes")
        
    # xls -> proto, table decoder in sharp
    resBuilder.Gen(inputs="excel/*.xlsx", src="xlsx", to="protodef", out_dir="out/excel/proto",
                   tabledecoder_dir="out/excel/tabledecoder")

    # proto -> python
    resBuilder.Gen(inputs="out/excel/proto/*.proto", src="proto", to="python", out_dir="out/excel/python")
    # proto -> csharp
    resBuilder.Gen(inputs="out/excel/proto/*.proto", src="proto", to="csharp", out_dir="out/excel/csharp", check="true")
    # proto -> cpp
    # resBuilder.Gen(inputs="out/excel/proto/*.proto", src="proto", to="cpp", out_dir="out/excel/cpp")
    # proto -> lua
    # resBuilder.Gen(inputs="out/excel/proto/*.proto", src="proto", to="lua", out_dir="out/excel/lua")

    resBuilder.Gen(inputs="excel/*.xlsx", src="xlsx", to="data", out_dir="out/excel/")

    # copy to project
    if exists_project_path:
        CopyFiles("out/excel/bin/*", ExcelConfigsPath)
        CopyFiles("out/excel/lua_table_out/*", LuaConfigsPath)
        CopyFiles("out/excel/csharp/*", ExcelSerializePath)
        CopyFiles("out/excel/tabledecoder/*", ExcelDecoderPath)
       


def process_json():
    # ::gen json
    if exists_project_path:
        if not os.path.exists("define/proto"):
            Mkdirs("define/proto")
        delete_file("define/proto", 1, "*.proto")

        CopyFiles(os.path.join(PROJECT_PATH, "SerializeCode/ProtobuffDes/*.proto"), "define/proto")
        CopyFiles(os.path.join(PROJECT_PATH, "SerializeCode/ProtobuffDes/Persistent/*.proto"), "define/proto")

    resBuilder.Gen(inputs="define/proto/*.proto", src="proto", to="python", out_dir="out/json/python")
    resBuilder.Gen(inputs="define/proto/*.proto", src="proto", to="csharp", out_dir="out/json/csharp", check="false")
    # resBuilder.Gen(inputs="define/proto/*.proto", src="proto", to="cpp", out_dir="out/json/cpp")
    resBuilder.Gen(inputs="define/proto/*.proto", src="proto", to="tabledecoder", out_dir="out/json/tabledecoder")
    resBuilder.Gen(inputs="out/json/python/*.py", src="py", to="json", out_dir="out/json/json_default_out")

    resBuilder.Check(inputs="define/json_config/*.json", py_dir="out/json/python")
    # call python gen_json_pbeans.py
    resBuilder.Gen(inputs="define/json_config/*.json", src="config", to="bytes", out_dir="out/json/bin",
                   py_dir="out/json/python",index="1");
    resBuilder.Check(inputs="define/json_config_2/*.json", py_dir="out/json/python")
    # call python gen_json_pbeans.py
    resBuilder.Gen(inputs="define/json_config_2/*.json", src="config", to="bytes", out_dir="out/json/bin",
                   py_dir="out/json/python",index="2");
    resBuilder.Check(inputs="define/json_config_3/*.json", py_dir="out/json/python")
    # call python gen_json_pbeans.py
    resBuilder.Gen(inputs="define/json_config_3/*.json", src="config", to="bytes", out_dir="out/json/bin",
                   py_dir="out/json/python",index="3");

    # call move_to_project.bat %PROJECT_PATH%   
    if exists_project_path:
        CopyFiles("out/json/bin/*", JsonlConfigsPath)
        CopyFiles("out/json/csharp/*", JsonSerializePath)
        CopyFiles("out/json/tabledecoder/*", JsonDecoderPath)


def process_hotfix_table():
    set_path()
    delete_Dic("out", 2)
    # xls -> proto, table decoder in sharp
    resBuilder.Gen(inputs="hotfix_excel/excel/*.xlsx", src="xlsx", to="protodef", out_dir="out/excel/proto",
                   tabledecoder_dir="out/excel/tabledecoder")
    # proto -> python
    resBuilder.Gen(inputs="out/excel/proto/*.proto", src="proto", to="python", out_dir="out/excel/python")
    # proto -> csharp
    resBuilder.Gen(inputs="out/excel/proto/*.proto", src="proto", to="csharp", out_dir="out/excel/csharp", check="true")
    resBuilder.Gen(inputs="hotfix_excel/excel/*.xlsx", src="xlsx", to="data", out_dir="out/excel/")

    resBuilder.Gen(inputs="define/proto/*.proto", src="proto", to="python", out_dir="out/json/python")
    resBuilder.Gen(inputs="out/json/python/*.py", src="py", to="json", out_dir="out/json/json_default_out")
    resBuilder.Check(inputs="hotfix_excel/json/json_config/*.json", py_dir="out/json/python")
    # call python gen_json_pbeans.py
    resBuilder.Gen(inputs="hotfix_excel/json/json_config/*.json", src="config", to="bytes", out_dir="out/json/bin",
                   py_dir="out/json/python",index="1")
    resBuilder.Check(inputs="hotfix_excel/json/json_config_2/*.json", py_dir="out/json/python")
    # call python gen_json_pbeans.py
    resBuilder.Gen(inputs="hotfix_excel/json/json_config_2/*.json", src="config", to="bytes", out_dir="out/json/bin",
                   py_dir="out/json/python",index="2")
    resBuilder.Check(inputs="hotfix_excel/json/json_config_3/*.json", py_dir="out/json/python")
    # call python gen_json_pbeans.py
    resBuilder.Gen(inputs="hotfix_excel/json/json_config_3/*.json", src="config", to="bytes", out_dir="out/json/bin",
                   py_dir="out/json/python",index="3")

    rename_hotfix_file_name()

def delete_old_file():
    if exists_project_path:
        print("wait for delete old file...")
        delete_Dic(ExcelSerializePath, 0)
        delete_Dic(ExcelDecoderPath, 0)
        delete_Dic(JsonSerializePath, 0)
        delete_Dic(JsonDecoderPath, 0)
        # delete_Dic(os.path.join(PROJECT_PATH, "SerializeCode/FbSerialize"), 0)
    delete_Dic("out", 2)
    print("delete old file success...")


def set_path():
    global ExcelSerializePath
    global ExcelDecoderPath
    
    global JsonSerializePath
    global JsonDecoderPath
    
    global ExcelConfigsPath
    global LuaConfigsPath
    global JsonlConfigsPath
    
    global PROJECT_PATH 
    global exists_project_path
    global CUSTOM_RUN
    if os.getenv("PROJECT_PATH"):
        PROJECT_PATH = os.getenv("PROJECT_PATH")
    if len(sys.argv) > 1:
        PROJECT_PATH = sys.argv[1]
    if os.path.exists("PathConfig.json"):
        path_configs = load_config_file("PathConfig.json")
        if "project_path" in path_configs \
                and path_configs["project_path"] != "" \
                and os.path.exists(path_configs["project_path"]):
            PROJECT_PATH = path_configs["project_path"]
        if "custom_run" in path_configs and path_configs["custom_run"]:
            CUSTOM_RUN = True
    else:
        PROJECT_PATH = (os.getcwd() + "/../../Game/Project")
    if not os.path.exists(PROJECT_PATH):
        exists_project_path = False
        print("没有配置转表路径，请检查PathConfig是否正确配置")
    print("PROJECT_PATH: " + PROJECT_PATH)
    
    ExcelSerializePath = os.path.join(PROJECT_PATH, "Assets/Scripts/Configs/ExcelSerialize")
    ExcelDecoderPath = os.path.join(PROJECT_PATH, "Assets/Scripts/Configs/ExcelDecoder")
   
    JsonSerializePath = os.path.join(PROJECT_PATH, "Assets/Scripts/Configs/JsonSerialize")
    JsonDecoderPath = os.path.join(PROJECT_PATH, "Assets/Scripts/Configs/JsonDecoder")
    
    ExcelConfigsPath = os.path.join(PROJECT_PATH, "Assets/Configs/ExcelConfigs")
    LuaConfigsPath = os.path.join(PROJECT_PATH, "Assets/Configs/LuaConfigs")
    JsonlConfigsPath = os.path.join(PROJECT_PATH, "Assets/Configs/JsonConfigs")

class Logger(object):
    def __init__(self, filename="Default.log"):
        self.terminal = sys.stdout
        self.log = open(filename, "w", encoding="utf-8")

    def write(self, message):
        self.terminal.write(message)
        self.log.write(message)

    def flush(self):
        self.terminal.flush()
        self.log.flush()


if __name__ == "__main__":
    if len(sys.argv) >= 2 and sys.argv[1] == "hotfix":
        process_hotfix_table()
    else:
        sys.stdout = Logger("Excel_Parse_Log.txt")
        set_path()
        if CUSTOM_RUN:
            custom_run(PROJECT_PATH)
        else:
            delete_old_file()
            process_xsl()
            process_json()
        print("Build config success")
       
