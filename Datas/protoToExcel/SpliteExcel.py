import os
import shutil
import xlrd,xlwt
import win32com.client as win32
import openpyxl

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

def ConvertXls2Xlsx():
    current_path = os.getcwd()
    source_folder_path = os.path.join(current_path, "excel")
    dst_folder_path = os.path.join(current_path, "dst_excel_xlsx")
    if not os.path.exists(dst_folder_path):
        os.mkdir(dst_folder_path)
    excel_files = os.listdir(source_folder_path)
    for file_name in excel_files:
        file_path = os.path.join(source_folder_path, file_name)
        dst_copy_file_path = os.path.join(dst_folder_path, file_name)
        # print(os.path.splitext(file_name)[-1])
        if is_chinese(file_name) or not (".xls" == os.path.splitext(file_name)[-1]):
            shutil.copyfile(file_path, dst_copy_file_path)
        else:
            shutil.copyfile(file_path, dst_copy_file_path)
            excel = win32.gencache.EnsureDispatch('excel.application')
            excel.Visible = False
            excel.DisplayAlerts = False
            print(file_name)
            print("xls转xlsx----->"+dst_copy_file_path)
            pro = excel.Workbooks.Open(dst_copy_file_path)  # 打开要转换的excel
            pro.SaveAs(dst_copy_file_path.replace(".xls",".xlsx"), FileFormat=51)  # 另存为xlsx格式
            pro.Close()
            excel.Application.Quit()
            os.remove(dst_copy_file_path)

def ConvertXlsx2Xls():
    current_path = os.getcwd()
    source_folder_path = os.path.join(current_path, "dst_excel")
    dst_folder_path = os.path.join(current_path, "dst")
    if not os.path.exists(dst_folder_path):
        os.mkdir(dst_folder_path)
    excel_files = os.listdir(source_folder_path)
    for file_name in excel_files:
        file_path = os.path.join(source_folder_path, file_name)
        dst_copy_file_path = os.path.join(dst_folder_path, file_name)
        # print(os.path.splitext(file_name)[-1])
        if is_chinese(file_name) or not (".xlsx" == os.path.splitext(file_name)[-1]):
            shutil.copyfile(file_path, dst_copy_file_path)
        else:
            shutil.copyfile(file_path, dst_copy_file_path)
            excel = win32.gencache.EnsureDispatch('excel.application')
            excel.Visible = False
            excel.DisplayAlerts = False
            print("xlsx转xls------->"+dst_copy_file_path)
            pro = excel.Workbooks.Open(dst_copy_file_path)  # 打开要转换的excel
            pro.SaveAs(dst_copy_file_path.replace(".xlsx", ".xls"), FileFormat=56)  # 另存为xlsx格式
            pro.Close()
            excel.Application.Quit()
            os.remove(dst_copy_file_path)

def SpliteExcels():
    current_path = os.getcwd()
    source_folder_path = os.path.join(current_path, "dst_excel_xlsx")
    dst_folder_path = os.path.join(current_path, "dst_excel")
    if not os.path.exists(dst_folder_path):
        os.mkdir(dst_folder_path)

    excel_files = os.listdir(source_folder_path)
    count = 0
    count2 = 0
    for file_name in excel_files:
        file_path = os.path.join(source_folder_path, file_name)
        dst_copy_file_path = os.path.join(dst_folder_path, file_name)
        if is_chinese(file_name) or not (".xlsx" == os.path.splitext(file_name)[-1]):
            shutil.copyfile(file_path, dst_copy_file_path)
        else:
            workbook = xlrd.open_workbook(file_path)
            sheet_names = workbook.sheet_names()
            for sheet_name in sheet_names:
                # shutil.copyfile(file_path, dst_copy_file_path)
                wb = openpyxl.load_workbook(file_path)
                for name in sheet_names:
                    if not(name == sheet_name):
                        wb.remove(wb[name])

                save_excel_path = ""
                if is_chinese(sheet_name):
                    count2 = count2+1
                    save_excel_path = file_name.replace(".xlsx", "") + "_" + sheet_name
                else:
                    count = count+1
                    save_excel_path = sheet_name
                save_path = os.path.join(dst_folder_path, save_excel_path)
                save_path = save_path + ".xlsx"
                wb.save(save_path)

                # 故意打开保存一下
                excel = win32.gencache.EnsureDispatch('excel.application')
                excel.Visible = False
                excel.DisplayAlerts = False
                print(save_path)
                pro = excel.Workbooks.Open(save_path)  # 打开要转换的excel
                pro.Save()
                pro.Close()
                excel.Application.Quit()
    print("分离出项目表格数量------->>"+str(count))
    print("分离出说明表格数量------->>" + str(count2))

if __name__ == "__main__":
    current_path = os.getcwd()
    path1 = os.path.join(current_path,"dst_excel_xlsx")
    path2 = os.path.join(current_path, "dst_excel")
    path3 = os.path.join(current_path, "dst")
    if os.path.exists(path1):
        shutil.rmtree(path1)
    if os.path.exists(path2):
        shutil.rmtree(path2)
    if os.path.exists(path3):
        shutil.rmtree(path3)
    ConvertXls2Xlsx()
    print("------>开始拆表")
    SpliteExcels()
    print("------>拆表完成")
    # print("------>开始转换表格")
    # ConvertXlsx2Xls()
    # if os.path.exists(path1):
    #     shutil.rmtree(path1)
    # if os.path.exists(path2):
    #     shutil.rmtree(path2)
    # print("------>操作完成 ，表格所在路径= " + os.path.join(current_path, "dst"))
