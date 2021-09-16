using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ClickMgr :BaseManager<ClickMgr>
{
    public ClickMgr()
    {
        MonoManager.GetInstance().AddUpdateListener(checkClick);
    }
    public void checkClick()
    {
        Debug.Log("触发点击");
        //左键按下时
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.transform != null)
            {
                Debug.Log(hit.transform.name);
            }
        }
        //右键按下时
        if (Input.GetMouseButtonDown(1))
        {

        }
    }
}
