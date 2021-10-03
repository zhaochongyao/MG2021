using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static string account;          //登录玩家的账号
    public GameObject player;   

    void Start()
    {
       
        MonoManager.GetInstance();
        InputMgr.GetInstance().changeInputStart(true);
        EventCenter.GetInstance().AddEventListener<Vector3>("点击事件",ClickObject);
     
    }
    void ClickObject(Vector3 clickPos)
    {
        var screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        clickPos = new Vector3(clickPos.x, clickPos.y, screenPosition.z);
        Debug.Log("mouse position = " + clickPos);
        Vector3 xxx = Camera.main.ScreenToWorldPoint(clickPos);
        Vector2 mousePos2D = new Vector2(xxx.x, xxx.y);
        Debug.Log("world position = " + xxx);
        var hit = Physics2D.OverlapPoint(mousePos2D);
        if (hit!=null)
        {
            Debug.Log(hit.name);
            hit.GetComponent<SpriteRenderer>().color = Color.black;
        }
    }
    
   
}
