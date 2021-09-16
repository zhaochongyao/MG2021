using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 用基类存储子类
/// </summary>
public interface IEventInfo { }
/// <summary>
/// 带有泛型的事件
/// </summary>
/// <typeparam name="T"></typeparam>
public class EventInfo<T>:IEventInfo
{
    public UnityAction<T> actions;
    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}
/// <summary>
/// 无泛型的事件
/// </summary>
public class EventInfo : IEventInfo
{
    public UnityAction actions;
    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}
/// <summary>
/// 事件中心
/// </summary>
public class EventCenter : BaseManager<EventCenter>
{
    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>(); 
    //添加事件
    public void AddEventListener<T>(string name,UnityAction<T> action){
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }
    //删除事件
    public void RemoveEventListener<T>(string name,UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions -= action;
        }
    }
  
    // 触发事件
    public void EventTrigger<T>(string name, T info)
    {
        
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions.Invoke(info);
        }
    }
    public void AddEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo(action));
        }
    }
    public void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions -= action;
        }
    }
    public void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions.Invoke();
        }
    }
}
