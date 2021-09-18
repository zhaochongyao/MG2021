using UnityEngine;

namespace Utilities.DesignPatterns
{
    /// <summary>
    /// 懒加载的单场景的泛型单例
    /// </summary>
    public class LSingletonLazy<T> : MonoBehaviour where T : LSingletonLazy<T> 
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
                }

                return _instance;
            }
        }
    }
}
