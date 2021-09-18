namespace Utilities.DesignPatterns
{
    /// <summary>
    /// 泛型单例模板，懒汉式
    /// 线程安全，有性能开销
    /// </summary>
    public class SingletonLazySafe<T> where T : SingletonLazySafe<T>, new()
    {
        /// <summary> 单例 </summary>
        private static T _instance;

        /// <summary> 加锁物体 </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object _locker = new object();
        // 每个子类单例都会拥有不同实例的locker，无冲突

        /// <summary> 私有化构造 </summary>
        private SingletonLazySafe() { }

        /// <summary> 获取单例 </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 当第一个线程运行到这里时，此时会对locker对象 "加锁"，
                    // 当第二个线程运行到这里时，检测到locker对象为"加锁"状态，
                    // 该线程就会挂起等待第一个线程解锁
                    // 第一个线程运行完之后, 会对该对象"解锁"
                    lock (_locker)
                    {
                        // 再判断一遍，防止第二个线程多次创建实例
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }

                return _instance;
            }
        }
    }
}
