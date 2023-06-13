from .gen_xls_to_proto import ConvertXls2Proto
from .gen_xls_to_data import ConvertXls2Data
from .gen_default_json import GenDefaultJson, GenJsonTableDecoder
from .gen_json_pbeans import GenerateJsonConfig
from .check_json_config import CheckAllJsonConfig
from .gen_excel_look_up import *
import os, glob, sys
from .util import *
from .hotfix import write_hotfix_lua_prefix
from .hotfix import write_hotfix_lua_suffix
from .generate_localization_assets import GenerateLocalizationAssets
from .merge_localization_keys import start_merge
battle_sheets = load_config_file("excel/BattleConfigLookup.json")["configs"]

def GetProtoExe():
    if os.name == "nt" or sys.platform == "win32" or sys.platform == "win64":
        return ".\\bin\\protoc.exe"
    else:
        return "protoc"

def GetFlatExe():
    if os.name == "nt" or sys.platform == "win32" or sys.platform == "win64":
        return ".\\bin\\flatc.exe"
    else:
        return "protoc"


def InvokeProtoc(**kv):
    to = kv["to"]
    out_dir = kv["out_dir"]    
    Mkdirs(out_dir)
    for filepath in glob.glob(kv["inputs"]):
        target = "" 
        if to == "python":
            target = "--python_out=" + out_dir
        elif to == "cpp":
            target = "--cpp_out=" + out_dir
        elif to == "csharp":
            if kv["check"] == "true":
                name = os.path.basename(filepath)[10:-6]
                if name not in battle_sheets:
                    continue
            target = "--csharp_out=" + out_dir
        elif to == "lua":
            target = "--plugin=protoc-gen-lua=resbuilder/protoc-gen-lua/plugin/protoc-gen-lua --lua_out=" + out_dir        
        else:
            raise BaseException("invalid type to " + kv["to"])
        proto_dir = os.path.dirname(filepath)
        cmd = "%s -I %s %s %s" % (GetProtoExe(), proto_dir, target, filepath)
        print(cmd)
        ret = os.system(cmd)
        if ret != 0:
            raise BaseException("call protc failed")

def InvokeFlatc(**kv):
    to = kv["to"]
    out_dir = kv["out_dir"]    
    Mkdirs(out_dir)
    for filepath in glob.glob(kv["inputs"]):
        target = "" 
        if to == "csharp":
            target = "-n -o " + out_dir
        else:
            raise BaseException("invalid type to " + kv["to"])
        proto_dir = os.path.dirname(filepath)
        cmd = "%s -I %s %s %s" % (GetFlatExe(), proto_dir, target, filepath)
        print(cmd)
        ret = os.system(cmd)
        if ret != 0:
            raise BaseException("call flatc failed")

def Gen(**kv):
    src = kv["src"]
    to = kv["to"]
    out_dir = kv["out_dir"]
    Mkdirs(out_dir)
    inputs = glob.glob(kv["inputs"])
    if src == "xlsx" and to == "protodef":
        for filepath in inputs:
            Mkdirs(kv["tabledecoder_dir"])
            ConvertXls2Proto(filepath, out_dir, kv["tabledecoder_dir"])

    elif src == "proto":
        if to == "tabledecoder":
            for filepath in inputs:
                GenJsonTableDecoder(filepath, out_dir)
        else:
            InvokeProtoc(**kv)
    elif src == "xlsx" and to == "data":
        write_hotfix_lua_prefix()
        for filepath in inputs:            
            Mkdirs(os.path.join(out_dir, "text"))
            Mkdirs(os.path.join(out_dir, "bin"))
            Mkdirs(os.path.join(out_dir, "lua_table_out"))
            Mkdirs(os.path.join(out_dir, "language_json_out"))
            Mkdirs(os.path.join(out_dir, "language_asset_out"))
            ConvertXls2Data(filepath, out_dir)
        write_hotfix_lua_suffix()
        _generate_localization = GenerateLocalizationAssets(inputs,os.path.join(out_dir, "language_asset_out"),
                                                            os.path.join(os.getcwd(), 'template.asset'),
                                                            os.path.join(os.getcwd(), "LocalizationKeyExcel"))
        # start_merge()
        _generate_localization.GenerateAssets()
    elif src == "fb":
        InvokeFlatc(**kv)
    elif src == "py" and to == "json":
        for filepath in inputs:            
            GenDefaultJson(filepath, out_dir)            
    elif src == "config" and to == "bytes":
        Mkdirs(out_dir)
        GenerateJsonConfig(kv["inputs"], out_dir, kv["py_dir"], kv["index"])
    else:
        raise BaseException("invalid file type %s => %s"% ( kv["src"], kv["to"]))


def Check(**kv):
    CheckAllJsonConfig(kv["inputs"],kv["py_dir"])