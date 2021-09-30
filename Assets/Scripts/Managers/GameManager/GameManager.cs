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
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(new Vector3(-11,2,0));
        Vector3 xxx = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, screenPoint.z));
        Vector2 mousePos2D = new Vector2(xxx.x, xxx.y);
        Collider2D[] colliders = Physics2D.OverlapPointAll(mousePos2D);
        if (colliders.Length > 0)
        {
            Debug.Log(colliders[0].gameObject.name);
        }
        EventCenter.GetInstance().EventTrigger("player_move_event", clickPos);
    }
    
   
}
