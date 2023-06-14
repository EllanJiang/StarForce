/*
* 文件名：Singleton
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/13 19:39:38
* 修改记录：
*/

using UnityEngine;

namespace GameMain
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

    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType(typeof(T)) as T;
                    if (_instance == null)
                    {
                        _instance = new GameObject("SingletonOf " + typeof(T).ToString(), typeof(T)).GetComponent<T>();
                        if (_instance == null)
                        {
                            Debug.LogError("Create Singleton Of" + typeof(T).ToString() + "Wrong!");
                        }
                    }

                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }

            if (_instance != null)
            {
                _instance.OnAwake();
            }
        }

        public static bool HasInstance()
        {
            return _instance != null;
        }
        
        //该函数用来初始化一些数据
        protected virtual void OnAwake()
        {
            
        }

        //确保在程序退出时销毁实例。
        private void OnApplicationQuit()
        {
            _instance = null;
        }
    }
}