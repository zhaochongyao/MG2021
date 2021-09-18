using UnityEngine;

namespace Utilities.DesignPatterns
{
    /// <summary>
    /// 全局的泛型单例、不受场景切换影响
    /// </summary>
    public abstract class GSingleton<T> : MonoBehaviour where T : GSingleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}