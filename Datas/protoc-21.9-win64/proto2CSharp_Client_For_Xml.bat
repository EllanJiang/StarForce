@echo off

rem curDir = D:\Learn\Unity_Learn\UGF\StarForce\Datas\protoc-21.9-win64\
set curDir=%~dp0

rem rootDir = D:\Learn\Unity_Learn\UGF\StarForce\
set rootDir=%curDir:Datas\protoc-21.9-win64\=%

rem echo %curDir%
rem echo %rootDir%

setlocal enabledelayedexpansion

rem ��ת��Э��洢Ŀ¼ ����Ҫ�������к������ע�ͣ����ױ���
cd %rootDir%Assets\GameMain\Scripts\Protos
set dir=XmlProtos
if not exist %dir% (
	md %dir%
)

rem ��ת��bat����Ŀ¼
cd %rootDir%Datas\protoc-21.9-win64
 
echo ��ʼת��.xml�ļ�


rem ����ע��
goto :comment
set index=0
for %%i in (xmls\*.xml) do (
	set /a index=index+1
	echo ��!index!���ļ�Ϊ%%i
    rem xml·��������·��
	bin\proto_xml\ProtoForXml.exe  %curDir%%%i %rootDir%Assets\GameMain\Scripts\Protos\XmlProtos
)

:comment
bin\proto_xml\ProtoForXml.exe  %curDir%xmls %rootDir%Assets\GameMain\Scripts\Protos\XmlProtos
echo Э��ת�����


setlocal disabledelayedexpansion

pause

