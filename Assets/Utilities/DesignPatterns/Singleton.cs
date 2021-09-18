namespace Utilities.DesignPatterns
{
    /// <summary>
    /// 泛型单例模板
    /// </summary>
    public class Singleton<T> where T : Singleton<T>, new()
    {
        // 在创建第一个实例或引用任何静态成员之前自动调用静态构造函数

        /// <summary> 单例 </summary>
        private static readonly T _instance = new T();

        /// <summary> 私有化构造 </summary>
        private Singleton() { }

        /// <summary> 获取单例 </summary>
        public static T Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
