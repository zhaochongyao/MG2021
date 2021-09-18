using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResourceMgr : BaseManager<ResourceMgr>
{
    /// <summary>
    /// 从Resources文件夹加载文件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">文件路径，不包括扩展名</param>
    /// <param name="isInstantiate">是否需要实例化</param>
    /// <returns></returns>
    public T Load<T>(string name, bool isInstantiate) where T:Object
    {
        T obj = Resources.Load<T>(name);
        if (obj is GameObject&&isInstantiate)
        {
            return GameObject.Instantiate(obj);
        }
        else
        {
            return obj;
        }
    }
    public void LoadAsyn<T>(string name,UnityAction<T> callback) where T : Object
    {
        MonoManager.GetInstance().StartCorourine(LoadAsynIE(name, callback));    
    }
    private IEnumerator LoadAsynIE<T>(string name,UnityAction<T> callback)where T:Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);
        yield return r;
        if (r.asset is GameObject)
        {
            callback((GameObject.Instantiate(r.asset)) as T);
        }
        else
        {
            callback(r.asset as T);        
        }
    }
}
