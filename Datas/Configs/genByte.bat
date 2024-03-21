rem set GEN_CLIENT=dotnet ..\Tools\Luban.ClientServer\Luban.ClientServer.dll
set GEN_CLIENT=..\Tools\Luban.ClientServer\Luban.ClientServer.exe

%GEN_CLIENT% -j cfg --^
 -d Defines\__root__.xml ^
 --input_data_dir Datas ^
 --output_data_dir ..\..\Assets\FileAssets\DataTables\LubanByteDatas ^
 --output_code_dir ..\..\Assets\GameMain\Scripts\LogicShared\LubanTables ^
 --gen_types code_cs_unity_bin,data_bin ^
 -s all
pause