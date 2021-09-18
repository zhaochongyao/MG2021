using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Layer
{
    Bot, Mid, Top, System
}
public class UIManager : BaseManager<UIManager> {
    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
    //Canvas中的层级
    private Transform bot;
    private Transform mid;
    private Transform top;
    private Transform system;
    //构造函数，同步加载Canvas和EventSystem
    public UIManager()
    {
        //同步方式加载Canvas，过场景后不删除Canvas
        GameObject obj_canvas = ResourceMgr.GetInstance().Load<GameObject>("UI/Canvas",true);
        Transform canvas = obj_canvas.transform;
        GameObject.DontDestroyOnLoad(obj_canvas);
        //获取Canvas中的各个层级
        bot = canvas.Find("Bot");
        mid = canvas.Find("Mid");
        top = canvas.Find("Top");
        system = canvas.Find("System");
        //同步方式加载EventSystem，过场景后不删除EventSystem
        GameObject obj_eventsystem = ResourceMgr.GetInstance().Load<GameObject>("UI/EventSystem",true);
        GameObject.DontDestroyOnLoad(obj_eventsystem);
    }
    //显示Panel
    public void ShowPanel<T>(string panelName, Layer layer, UnityAction<T> callback = null) where T : BasePanel
    {
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].ChangePanelState(true);
            return;
        }
        //异步加载Panel
        ResourceMgr.GetInstance().LoadAsyn<GameObject>("UI/" + panelName, (obj) =>
        {
            //设置在哪个层级之下，初始化它的位置，大小，偏移量
            Transform father = bot;
            switch (layer)
            {
                case Layer.Mid:
                    father = mid;
                    break;
                case Layer.Top:
                    father = top;
                    break;
                case Layer.System:
                    father = system;
                    break;
            }
            obj.transform.SetParent(father);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            (obj.transform as RectTransform).offsetMax = new Vector2(310, 720);
            (obj.transform as RectTransform).offsetMin = new Vector2(-310, -720);
          //  (obj.transform as RectTransform).offset
            //获取Panel身上的脚本
            T panel = obj.GetComponent<T>();
            if (callback != null)
                callback(panel);
            //添加到字典中
            panelDic.Add(panelName, panel);
        });
    }
   
    //隐藏Panel
    public void HidePanel(string panelName)
    {
        if (panelDic.ContainsKey(panelName))
        {
           
            panelDic[panelName].ChangePanelState(false);
            //删除物体并从字典中移除
         //   GameObject.Destroy(panelDic[panelName].gameObject);
         //   panelDic.Remove(panelName);
        }
    }

}
