# encoding: utf8
# !/usr/bin/python
import json
import os.path, shutil, glob
import time
import hashlib

config_dic = {}

# 递归创建多层目录
def Mkdirs(path):
    if os.path.exists(path):
        return
    parent = os.path.dirname(path)
    if parent != "" and not os.path.exists(parent):
        Mkdirs(parent)
    os.mkdir(path)


# 拷贝多个文件
# CopyFiles("a/*.py", "b")
def CopyFiles(srcpattern, dstdir):
    Mkdirs(dstdir)
    for filepath in glob.glob(srcpattern):
        basename = os.path.basename(filepath)
        shutil.copy(filepath, os.path.join(dstdir, basename))


#删除指定后缀名文件
def delete_file(path, t, sign="*.bytes"):
    try:
        if not os.path.exists(path):
            return
        # shutil.rmtree(path)
        for file_path in glob.glob(os.path.join(path, sign)):
            os.remove(os.path.join(file_path))
        time.sleep(t)
    except BaseException as e:
        raise e


def delete_Dic(path, t):
    try:
        if not os.path.exists(path):
            return
        shutil.rmtree(path)
        time.sleep(t)
    except BaseException as e:
        raise e


def load_config_file(name):
    try:
        if not os.path.exists(name):
            return
        if name in config_dic:
            return config_dic[name]
        f = open(name, encoding='utf-8')
        config = json.load(f)
        config_dic[name] = config
        return config
    except BaseException as e:
        raise e

def is_chinese(uchar):
    if uchar >= u'\u4e00' and uchar <= u'\u9fa5':
        return True
    else:
        return False

def contain_chinese(str):
    for uchar in str:
        if(is_chinese(uchar)):
            return True
    return False

 # md5后9位数字
def get_str_md5_9(parmStr):
    parmStr = parmStr.encode("utf-8")
    m = hashlib.md5()
    m.update(parmStr)
    print("m.hexdigest():",m.hexdigest())
    # m.hexdigest()十六进制表示
    result = int(m.hexdigest(), 16) #十六进制表示转成10进制表示
    print("result:",result)
    result = str(result)[-9:]
    print("result2:",result)
    return result

# md5后4位数字
def get_str_md5_4(parmStr):
    parmStr = parmStr.encode("utf-8")
    m = hashlib.md5()
    m.update(parmStr)
    result = int(m.hexdigest(), 16)
    result = str(result)[-4:]
    return result
    
    