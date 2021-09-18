using UnityEngine;

namespace Utilities.DesignPatterns
{
    /// <summary>
    /// 懒加载的全局泛型单例
    /// </summary>
    public class GSingletonLazy<T> : MonoBehaviour where T : GSingletonLazy<T>
    {
        private static T _instance;

        public static T Instance 
        { 
            get
            {
                if (_instance == null)
                {
                    GameObject lazy = new GameObject();
                    _instance = lazy.AddComponent<T>();
                    lazy.name = typeof(T).Name;

                    DontDestroyOnLoad(lazy);
                }

                return _instance;
            }
        }
    }
}
