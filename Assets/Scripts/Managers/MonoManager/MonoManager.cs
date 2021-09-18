using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoManager : BaseManager<MonoManager>
{
    private MonoController controller;
    public MonoManager()
    {
        GameObject obj = new GameObject("MonoController");
        controller = obj.AddComponent<MonoController>();
    }
    public void AddUpdateListener(UnityAction func)
    {
        controller.AddUpdateListener(func);
    }
    public void RemoveUpdateListener(UnityAction func)
    {
        controller.RemoveUpdateListener(func);
    }
    public Coroutine StartCorourine(IEnumerator routine)
    {
        return controller.StartCoroutine(routine);
    }
    public void AddFixedUpdateListener(UnityAction func)
    {
        controller.AddFixedUpdateListener(func);
    }
    public void RemoveFixedUpdateListener(UnityAction func)
    {
        controller.RemoveFixedUpdateListener(func);
    }

}
