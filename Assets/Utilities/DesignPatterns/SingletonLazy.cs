namespace Utilities.DesignPatterns
{
    /// <summary>
    /// 泛型单例模板，懒汉式
    /// </summary>
    public class SingletonLazy<T> where T : SingletonLazy<T>, new()
    {
        /// <summary> 单例 </summary>
        private static T _instance;

        /// <summary> 私有化构造 </summary>
        protected SingletonLazy() { }

        /// <summary> 获取单例 </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }

                return _instance;
            }
        }
    }
}
