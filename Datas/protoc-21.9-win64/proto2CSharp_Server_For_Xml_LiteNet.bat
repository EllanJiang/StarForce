@echo off

set curDir=%~dp0
set rootDir=%curDir:Datas\protoc-21.9-win64\=%

setlocal enabledelayedexpansion

rem ��ת����ǰĿ¼ ����Ҫ�������к������ע�ͣ����ױ���
cd D:\Learn\Unity_Learn\GameFramework_Server\Proto\XmlProtosForLiteNet
set dir=XmlProtosForLiteNet
if not exist %dir% (
	md %dir%
)
cd %rootDir%Datas\protoc-21.9-win64

rem ��ת��bat����Ŀ¼
 
echo ��ʼת��.xml�ļ�

:: xml�����ļ��� �����ļ���
bin\proto_xmlForLiteNet\ProtoForXml.exe  %curDir%xmlsForLiteNet D:\Learn\Unity_Learn\GameFramework_Server\Proto\XmlProtosForLiteNet

echo Э��ת�����


setlocal disabledelayedexpansion

pause

