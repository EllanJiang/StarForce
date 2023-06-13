# encoding: utf8
# !/usr/bin/python
import json
import os.path, shutil, glob

look_up_output = []


def add_excel_line(line):
    ls = str.split(line, '\\')
    look_up_output.append(ls[1] + "\n")


def add_sheet_line(line):
    if is_chinese(line):
        return
    look_up_output.append("\t" + line + "\n")


def save_excel_look_up(out_dir):
    try:
        name = os.path.join(out_dir, "excel_lookup.txt")
        f = open(name, 'w+')
        f.writelines(look_up_output)
        f.close()
    except BaseException as e:
        print("open excel_lookup file(%s) failed!" % out_dir)
        raise


def is_chinese(string):
    """
    检查整个字符串是否包含中文
    :param string: 需要检查的字符串
    :return: bool
    """
    for ch in string:
        if u'\u4e00' <= ch <= u'\u9fff':
            return True
    return False
