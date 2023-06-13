# -*- coding: utf8 -*-'
import os, os.path
import time
import shutil

cdn_path = "rsync://cdn@10.234.36.134/cdn/dsconfig"
root_path = ""
protocol_path = ""

def ReadConfig(fileName):
    global protocol_path
    if not os.path.exists(fileName):
        return {}
    cfgFile = open(fileName)
    cfg = {}
    for line in cfgFile.readlines():
        strs = line.strip().split("=")
        if len(strs) == 2:
            cfg[strs[0].strip()] = strs[1].strip()
    if cfg != None:
        protocol_path = cfg["PROTOCOL_PATH"]

# 生成上传配置，
def GenImportConfig():
    t = int(time.time())
    t = str(t)
    import_dir = os.path.join(os.getcwd() + "/cdn/dsconfig", t + "/ds_Data/StreamingAssets/ConfigAsset")
    print("gen config start============================")
    xls_bin_path = protocol_path + "/excel/bin"
    json_bin_path = protocol_path + "/json/bin"
    
    # 生成时间戳文件夹
    os.makedirs(import_dir)
    file = open(import_dir + "/file_config.txt", "w")
    for xls_name in os.listdir(xls_bin_path):
        print(xls_name)
        file.write("XlsConfig/" + xls_name + "\n")
    print("-------------------------------")
    for json_name in os.listdir(json_bin_path):
        print(json_name)
        file.write("JsonBinConfig/" + json_name + "\n")
    file.flush()
    file.close()
    shutil.copytree(xls_bin_path, import_dir + "/" + "XlsConfig")
    shutil.copytree(json_bin_path, import_dir + "/" + "JsonBinConfig")
    print("gen config end============================")
    return t


def Start():
    # 读取配置
    # ReadConfig("config.txt")
    global protocol_path
    protocol_path = os.getcwd() + "/protocol/out"
    protocol_path = protocol_path.replace("\\", "/")
    if not os.path.isdir(protocol_path):
        print("不存在上传路径：" + protocol_path)
        par_path = os.path.abspath(os.path.dirname(os.getcwd()))
        protocol_path = par_path + "/out"

    protocol_path = protocol_path.replace("\\", "/")
    if protocol_path.find("/protocol/out") < 0 and (not os.path.isdir(protocol_path)):
        print("不存在上传路径：" + protocol_path)
        return

    print("使用protocol路径：" + protocol_path)
    
    # 设置根路径
    global root_path
    root_path = os.getcwd().replace("\\", "/")

    # 同步cdn配置
    rsync_path = root_path + "/code/cwrsync_6_2_0_x64_free/bin"
    os.chdir(rsync_path)
    if os.path.isdir(os.getcwd() + "/cdn/"):
        shutil.rmtree(os.getcwd() + "/cdn/")
    cmd = "rsync.exe --password-file=./dsconfig.key" + " -avP --delete --chmod=a=rwx " + cdn_path + " cdn/"
    os.system(cmd)
    print(cmd)

    # 生成文件
    version = GenImportConfig()
    
    # 上传cdn
    cmd = "rsync.exe --password-file=./dsconfig.key" + " -av --delete cdn/dsconfig/ " + cdn_path
    os.system(cmd)
    print(cmd)
    shutil.rmtree(os.getcwd() + "/cdn/")

    # 版本号写进文本
    print("生成版本:" + version)
    file = open(root_path + "/DSConfigVersion.txt", "w")
    file.write("DSConfigVersion：" + version)
    file.flush()
    file.close()

Start()
os.system("pause")