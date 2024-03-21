/*
* 文件名：Singleton
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/13 19:39:38
* 修改记录：
*/


namespace LogicShared
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static object _syncobj = new object();
        private static volatile T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncobj)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }

                return _instance;
            }
        }

        public static bool HasInstance()
        {
            return _instance != null;
        }
        
        protected virtual void Dispose()
        {
        }

        public void Destroy()
        {
            Dispose();
            _instance = null;
        }
    }
    
}