#!\\usr\\bin\\python
# sys.setdefaultencoding('utf-8')
import os, glob, sys, json
from google.protobuf import json_format

def check_data(data_file, _default_data):
    _field_to_del = []
    try:
        _data_file = open(data_file, 'r', encoding='utf-8')
        _json_obj = json.load(_data_file.buffer)
        _data_file.close()
    except BaseException as e:
        print(data_file)
        raise e
    '''批量修改json范例
    if 'parent' in _json_obj:
        if 'PickUpAssetName' in _json_obj['parent']:
            str_temp = _json_obj['parent']['MeshAssetName1P']
            index = str_temp.find('_')
            _json_obj['parent']['PickUpAssetName'] = str_temp[0:index] + "_pickup"
    '''
    save(data_file, _default_data, _json_obj)

def save(file_name, default, result):
    _data = json_format.ParseDict(result, default, True)
    _json_result = json_format.MessageToJson(_data, True, True, 2, False, False, None, 5, False)
    try:
        with open(file_name, 'w+', encoding='utf-8') as file:
            file.write(_json_result)
    except BaseException as e:
        raise e

def CheckAllJsonConfig(path, module_path):
    try:
        sys.path.append(module_path)
        for file in glob.glob(module_path + '/*.py'):
            _module_name = os.path.splitext(file)[0].split('\\')[-1]
            if _module_name == 'PbVector_pb2':
                continue
            exec('import ' + _module_name)

        for data_file in glob.glob(path):
            try:
                _data_name = os.path.splitext(data_file)[0].split('\\')[-1].split('=')[0]
                _default_data = eval(_data_name + '_pb2.' + _data_name + '()')
                check_data(data_file, _default_data)
            except BaseException as e:
                print("CheckAllJsonConfig lack .proto,error file : " + data_file)

    except BaseException as e:
        raise e

if __name__ == "__main__":
    path = os.getcwd()
    CheckAllJsonConfig(path + "/define/json_config/*.json", path + "/out/json/python")
