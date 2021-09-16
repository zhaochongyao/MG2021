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
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(clickPos);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider != null)
        {
            Debug.Log(hit.transform.name);
        }
    }
    
   
}
