using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BasePanel : MonoBehaviour
{
    private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();
    private void Awake()
    {
        FindChildControl<Button>();
        FindChildControl<Image>();
        FindChildControl<Slider>();
        FindChildControl<Toggle>();
        FindChildControl<Text>();
        FindChildControl<InputField>();
    }
    protected void FindChildControl<T>()where T : UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>();
        for (int i = 0; i < controls.Length; ++i)
        {
            string controlName = controls[i].gameObject.name;
           
            if (controlDic.ContainsKey(controlName))
                controlDic[controlName].Add(controls[i]);
            else
                controlDic.Add(controlName, new List<UIBehaviour>() { controls[i] });
        }
        
    }
    protected T GetControl<T>(string name) where T : UIBehaviour
    {
        if (controlDic.ContainsKey(name))
        {
            for (int i = 0; i < controlDic[name].Count; ++i)
            {
                if (controlDic[name][i] is T)
                    return (controlDic[name][i] as T);
            }
        }
        return null;
    }
    /// <summary>
    /// 更改panel显示和隐藏
    /// </summary>
    /// <param name="show">是否显示</param>
    public void ChangePanelState(bool show)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup!=null)
        {
            canvasGroup.alpha = show?1:0;
            canvasGroup.interactable = show;
            canvasGroup.blocksRaycasts = show;
        }
    }
    
}
