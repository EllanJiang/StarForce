@echo off

set curDir=%~dp0
set rootDir=%curDir:Datas\protoc-21.9-win64\=%

setlocal enabledelayedexpansion

rem ��ת����ǰĿ¼ ����Ҫ�������к������ע�ͣ����ױ���
cd D:\Learn\Unity_Learn\GameFramework_Server\Proto\XmlProtos
set dir=XmlProtos
if not exist %dir% (
	md %dir%
)
cd %rootDir%Datas\protoc-21.9-win64

rem ��ת��bat����Ŀ¼
 
echo ��ʼת��.xml�ļ�

:: xml�����ļ��� �����ļ���
bin\proto_xml\ProtoForXml.exe  %curDir%xmls D:\Learn\Unity_Learn\GameFramework_Server\Proto\XmlProtos

echo Э��ת�����


setlocal disabledelayedexpansion

pause

