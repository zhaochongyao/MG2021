using System.Collections;
using System.Collections.Generic;
using Managers.AchieveManager;
using Managers.UImanager;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;   

    void Start()
    {
       
        MonoManager.GetInstance();
        InputMgr.GetInstance().changeInputStart(true);
        EventCenter.GetInstance().AddEventListener<Vector3>("点击事件",ClickObject);
        // UIManager.GetInstance().ShowPanel<GameStartPanel>("GameStartPanel", Layer.System, null);
        // AchievementMgr.GetInstance().Init();
    }
    void ClickObject(Vector3 clickPos)
    {
        // var screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        // clickPos = new Vector3(clickPos.x, clickPos.y, screenPosition.z);
        // Debug.Log("mouse position = " + clickPos);
        // Vector3 xxx = Camera.main.ScreenToWorldPoint(clickPos);
        // Vector2 mousePos2D = new Vector2(xxx.x, xxx.y);
        // Debug.Log("world position = " + xxx);
        // var hit = Physics2D.OverlapPoint(mousePos2D);
        // if (hit!=null)
        // {
        //     Debug.Log(hit.name);
        //     hit.GetComponent<SpriteRenderer>().color = Color.black;
        // }
    }
    
   
}
