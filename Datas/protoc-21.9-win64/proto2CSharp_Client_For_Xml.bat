@echo off

rem curDir = D:\Learn\Unity_Learn\UGF\StarForce\Datas\protoc-21.9-win64\
set curDir=%~dp0

rem rootDir = D:\Learn\Unity_Learn\UGF\StarForce\
set rootDir=%curDir:Datas\protoc-21.9-win64\=%

rem echo %curDir%
rem echo %rootDir%

setlocal enabledelayedexpansion

rem 跳转到协议存储目录 ，不要在命令行后面添加注释，容易报错
cd %rootDir%Assets\GameMain\Scripts\Protos
set dir=XmlProtos
if not exist %dir% (
	md %dir%
)

rem 跳转该bat所在目录
cd %rootDir%Datas\protoc-21.9-win64
 
echo 开始转化.xml文件


rem 多行注释
goto :comment
set index=0
for %%i in (xmls\*.xml) do (
	set /a index=index+1
	echo 第!index!个文件为%%i
    rem xml路径，保存路径
	bin\proto_xml\ProtoForXml.exe  %curDir%%%i %rootDir%Assets\GameMain\Scripts\Protos\XmlProtos
)

:comment
bin\proto_xml\ProtoForXml.exe  %curDir%xmls %rootDir%Assets\GameMain\Scripts\Protos\XmlProtos
echo 协议转换完成


setlocal disabledelayedexpansion

pause

