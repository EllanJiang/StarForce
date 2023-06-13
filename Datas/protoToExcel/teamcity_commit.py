import os

def commit_dir(dir_path, logStr):
    if not os.path.exists(dir_path):
        return

    os.chdir(dir_path)
    svn_status = os.popen('svn st')
    svn_info = svn_status.readlines()
    for line in svn_info:
        line = line.strip('\n').split('       ')
        status = line[0]
        path = line[1]
        if status == '?':
            os.system(f'svn add {path}' )
        elif status == '!':
            os.system(f'svn delete {path}')

    os.system(f"svn commit . -m \"{logStr}\"")


commit_dir("out", "CI:Auto Generate Config")
