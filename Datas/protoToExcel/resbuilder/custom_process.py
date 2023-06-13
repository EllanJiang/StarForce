import os
import sys

import resbuilder as resBuilder
from resbuilder.util import *
from resbuilder.hotfix import rename_hotfix_file_name

def custom_run(PROJECT_PATH):
    input_path = "custom_run_input"
    output_path = "custom_run_output"
    if not os.path.exists(input_path):
        os.makedirs(input_path)
        print("The folder for the input data could not be found. ")
        print("The system has automatically created the folder. ")
        print("Please put the excel files that need to be processed into this folder.")
        sys.exit()
    if not os.path.exists(output_path):
        os.makedirs(output_path)
        print("The folder for the output data could not be found. ")
        print("The system has automatically created the folder. ")
        delete_Dic(output_path, 1)
    resBuilder.Gen(inputs=input_path+"/*.xlsx", src="xlsx", to="protodef", out_dir=output_path+"/proto",
                   tabledecoder_dir=output_path+"/tabledecoder")
    resBuilder.Gen(inputs=output_path+"/proto/*.proto", src="proto", to="python", out_dir=output_path+"/python")
    resBuilder.Gen(inputs=output_path+"/proto/*.proto", src="proto", to="csharp", out_dir=output_path+"/csharp", check="true")
    resBuilder.Gen(inputs=input_path+"/*.xlsx", src="xlsx", to="data", out_dir=output_path)


    if PROJECT_PATH != "" \
        and os.path.exists(PROJECT_PATH):
            CopyFiles(output_path+"/bin/*", os.path.join(PROJECT_PATH, "Assets/Configs/ExcelConfigReal"))
            ds_path = os.path.join(PROJECT_PATH, "Assets/Configs/ExcelConfigReal")
            ds_path = ds_path.replace("ConfigAsset", "StreamingAssets/ConfigAsset")
            if os.path.exists(ds_path):
                CopyFiles(output_path+"/bin/*", ds_path)
            CopyFiles(output_path+"/lua_table_out/*", os.path.join(PROJECT_PATH, "Assets/Configs/LuaConfigReal"))
            CopyFiles(output_path+"/csharp/*", os.path.join(PROJECT_PATH, "Assets/Scripts/RealConfig/ExcelSerialize"))
            CopyFiles(output_path+"/tabledecoder/*", os.path.join(PROJECT_PATH, "Assets/Scripts/RealConfig/ExcelDecoder"))
