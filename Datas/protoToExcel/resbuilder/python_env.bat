REM download python
curl https://www.python.org/ftp/python/3.7.6/python-3.7.6-amd64.exe  -o python-3.7.6-amd64.exe

REM install python，安装时需要勾选将python所在目录加入PATH
python-3.7.6-amd64.exe

REM 安装依赖工具
pip install xlrd
pip install protobuf
pip install pyinstaller



