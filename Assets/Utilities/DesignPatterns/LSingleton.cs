using UnityEngine;

namespace Utilities.DesignPatterns
{
    /// <summary>
    /// 单场景的泛型单例
    /// </summary>
    public abstract class LSingleton<T> : MonoBehaviour where T : LSingleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            Instance = this as T;
        }
    }
}
