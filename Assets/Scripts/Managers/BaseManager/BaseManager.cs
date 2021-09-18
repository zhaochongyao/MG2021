

using System;
/// <summary>
/// 单例模式基类
/// 加锁懒汉模式
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseManager<T> where T:new()
{
    private static T instance;              //单例对象
    private static readonly Object lock_Object = new object(); //加锁对象
    public static T GetInstance()
    {
        if (instance == null)                   
        {
            lock (lock_Object)                  //加锁使线程安全  防止多个线程同时创建instance对象
            {
                if (instance == null)
                {
                    instance = new T();
                }
            }
        }
        return instance;
    }


}
