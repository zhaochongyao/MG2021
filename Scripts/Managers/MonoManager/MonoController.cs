using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoController : MonoBehaviour
{
    private event UnityAction action;
    private event UnityAction phyics_action;
  
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (action != null)
        {
            action();
        }
        
    }
    private void FixedUpdate()
    {
        if (phyics_action != null)
        {
            phyics_action();
        }
    }
    public void AddUpdateListener(UnityAction func)
    {
        action += func;
    }
    public void RemoveUpdateListener(UnityAction func)
    {
        action -= func;
    }
    public void AddFixedUpdateListener(UnityAction func)
    {
        phyics_action += func;
    }
    public void RemoveFixedUpdateListener(UnityAction func)
    {
        phyics_action -= func;
    }
}
