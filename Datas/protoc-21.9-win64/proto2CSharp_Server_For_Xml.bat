@echo off

set curDir=%~dp0
set rootDir=%curDir:Datas\protoc-21.9-win64\=%

setlocal enabledelayedexpansion

rem 跳转到当前目录 ，不要在命令行后面添加注释，容易报错
cd D:\Learn\Unity_Learn\GameFramework_Server\Proto\XmlProtos
set dir=XmlProtos
if not exist %dir% (
	md %dir%
)
cd %rootDir%Datas\protoc-21.9-win64

rem 跳转该bat所在目录
 
echo 开始转化.xml文件

:: xml所在文件夹 保存文件夹
bin\proto_xml\ProtoForXml.exe  %curDir%xmls D:\Learn\Unity_Learn\GameFramework_Server\Proto\XmlProtos

echo 协议转换完成


setlocal disabledelayedexpansion

pause

