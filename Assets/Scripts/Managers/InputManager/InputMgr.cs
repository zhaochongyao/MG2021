using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMgr : BaseManager<InputMgr>
{
    private bool isStart = false;
    public InputMgr()
    {
        MonoManager.GetInstance().AddUpdateListener(InputUpdate);
    }
    /// <summary>
    /// 修改输入状态是否开启
    /// </summary>
    /// <param name="start"></param>
    public void changeInputStart(bool start)
    {
        isStart = start;
    }
    
    public void CheckKeyCode(KeyCode key)
    {
       
        if (Input.GetKey(key))
        {
            EventCenter.GetInstance().EventTrigger<KeyCode>("按键按下", key);
        }
        else if(Input.GetKeyUp(key))
        {
            EventCenter.GetInstance().EventTrigger<KeyCode>("按键松开", key);
        }
    }
    public void checkClick()
    {
        //左键按下时
        if (Input.GetMouseButtonDown(0))
        {
            EventCenter.GetInstance().EventTrigger<Vector3>("点击事件", Input.mousePosition);
        }
        // 左键松开
        if (Input.GetMouseButtonUp(0))
        {
            EventCenter.GetInstance().EventTrigger<Vector3>("左键松开", Input.mousePosition);
        }
        //右键按下时
        if (Input.GetMouseButtonDown(1))
        {
            EventCenter.GetInstance().EventTrigger<Vector3>("右键点击", Input.mousePosition);
        }
        if (Input.touchCount == 1)
        {
            if(Input.touches[0].phase == TouchPhase.Began)              //手指按下
            {
                EventCenter.GetInstance().EventTrigger<Vector3>("点击事件",Input.touches[0].position);
            }
        }
    }
    public void InputUpdate()
    {
        if (isStart)
        {
            //检测所有有效输入
            CheckKeyCode(KeyCode.A);
            CheckKeyCode(KeyCode.D);
            CheckKeyCode(KeyCode.W);
            CheckKeyCode(KeyCode.S);
            checkClick();
        }
    }
}
