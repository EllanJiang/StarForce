import os

os.system("build_all_config_ci.bat")
with open("res_gen_log.txt", 'rb') as f_log_file:
    log_str = f_log_file.read().decode("utf-8")
    if "Build config success" not in log_str:
        print(log_str)
        raise Exception(f"Generate Config Failed")