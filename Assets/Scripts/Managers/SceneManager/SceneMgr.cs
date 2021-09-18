using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneMgr :BaseManager<SceneMgr>
{
    /// <summary>
    /// 加载模块
    /// </summary>
    public void LoadScene(string name,UnityAction action)
    {
        SceneManager.LoadScene(name);
        action();
    }
    /// <summary>
    /// 异步加载模块
    /// </summary>
    /// <param name="name"></param>
    /// <param name="aciton"></param>
    public void LoadSceneAsyn(string name,UnityAction action)
    {
        MonoManager.GetInstance().StartCorourine(LoadSceneAsynIE(name, action));
    }

    private IEnumerator LoadSceneAsynIE(string name,UnityAction aciton)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        while (!ao.isDone)      //未加载完时更新UI进度条等等
        {

            yield return ao.progress;
        }
        aciton();
    }
}
