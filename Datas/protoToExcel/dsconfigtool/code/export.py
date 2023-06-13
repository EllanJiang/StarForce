# -*- coding: utf8 -*-'
import os, os.path
import shutil

svn_path = "http://10.234.36.132/alt/trunk/protocol"

def Start():
    r = input("请输入svn版本号：")
    export_path = os.getcwd() + "/protocol"
    export_path = export_path.replace("\\", "/")
    if os.path.isdir(export_path):   
        shutil.rmtree(export_path)
    cmd = "svn export -r " + r + " " + svn_path + " " + export_path
    print(cmd)
    os.system(cmd)

Start()
os.system("pause")