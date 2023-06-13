echo off 
cd ..
echo %cd%

pyinstaller -F .\resbuilder\replace_localization_keys.py -p .\resbuilder\
pyinstaller -F .\resbuilder\merge_localization_keys.py -p .\resbuilder\
pyinstaller -F .\resbuilder\validation_excel_localization.py -p .\resbuilder\