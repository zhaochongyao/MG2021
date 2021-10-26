using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utilities.DataStructures;

// ReSharper disable PossibleNullReferenceException

namespace Utilities
{
    /// <summary>
    /// 场景加载器
    /// </summary>
    public static class SceneLoader
    {
        /// <summary> 进度条更新事件 </summary>
        public static event Action<float> ProgressUpdate;
        
        /// <summary> 是否正在加载 </summary>
        public static bool IsLoading { get; private set; }

        /// <summary> 加载进度 </summary>
        public static float Progress { get; private set; }

        /// <summary> 当前场景 </summary>
        public static string ActiveScene => SceneManager.GetActiveScene().name;


        /// <summary> 预加载事件 </summary>
        public static event Action<string> PreLoad;

        /// <summary> 异步预加载事件 </summary>
        public static event Action<string> PreLoadAsync;

        /// <summary> 异步预加载完成判定 </summary>
        private static HashSet<Func<bool>> _preLoadReady;

        /// <summary> 异步完成判定绑定 </summary>
        /// <param name="ready"> 判定函数 </param>
        public static void PreLoadReadyConnect(Func<bool> ready)
        {
            _preLoadReady.Add(ready);
        }

        /// <summary> 异步完成判定解绑 </summary>
        /// <param name="ready"> 判定函数 </param>
        public static void PreLoadReadyDisconnect(Func<bool> ready)
        {
            _preLoadReady.Remove(ready);
        }
        
        /// <summary> 预处理事件 </summary>
        public static event Action<string> PreProc;

        /// <summary> 异步预处理事件 </summary>
        public static event Action<string> PreProcAsync;

        /// <summary> 异步预处理完成判定 </summary>
        private static HashSet<Func<bool>> _preProcReady;
        
        /// <summary> 异步完成判定绑定 </summary>
        /// <param name="ready"> 判定函数 </param>
        public static void PreProcReadyConnect(Func<bool> ready)
        {
            _preProcReady.Add(ready);
        }

        /// <summary> 异步完成判定解绑 </summary>
        /// <param name="ready"> 判定函数 </param>
        public static void PreProcReadyDisconnect(Func<bool> ready)
        {
            _preProcReady.Remove(ready);
        }

        /// <summary> 当前场景更改事件 </summary>
        public static event UnityAction<Scene, Scene> ActiveSceneChanged;

        /// <summary> 场景加载完毕事件 </summary>
        public static event UnityAction<Scene, LoadSceneMode> SceneLoaded;

        /// <summary> 场景卸载完毕事件 </summary>
        public static event UnityAction<Scene> SceneUnloaded; 
        
        // 生命周期顺序为
        // Awake -- OnEnable -- ActiveSceneChanged -- SceneLoaded
        // Start -- OnDisable -- OnDestroy -- SceneUnloaded

        /// <summary> 初始化 </summary>
        internal static void Init()
        {
            IsLoading = false;
            Progress = 1.0f;
            // 绑定属性的更新
            static void SetProgress(float progress) => Progress = progress;
            ProgressUpdate += SetProgress;
            
            PreLoad = delegate { };
            PreProc = delegate { };
            ProgressUpdate = delegate { };
            PreProcAsync = delegate { };
            _preProcReady = new HashSet<Func<bool>>();
            _preLoadReady = new HashSet<Func<bool>>();

            static void ChangeScene(Scene lhs, Scene rhs) => ActiveSceneChanged?.Invoke(lhs, rhs);

            static void Loaded(Scene scene, LoadSceneMode mode) => SceneLoaded?.Invoke(scene, mode);

            static void Unloaded(Scene scene) => SceneUnloaded?.Invoke(scene);
            
            // 复用SceneManager的事件方法
            SceneManager.activeSceneChanged += ChangeScene;
            SceneManager.sceneLoaded += Loaded;
            SceneManager.sceneUnloaded += Unloaded;
        }

        /// <summary> 确保预加载全部完成 </summary>
        private static bool PreLoadFinished()
        {
            foreach(Func<bool> ready in _preLoadReady)
            {
                if (ready() == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary> 确保预处理全部完成 </summary>
        private static bool PreProcFinished()
        {
            foreach(Func<bool> ready in _preProcReady)
            {
                if (ready() == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary> 异步加载场景 </summary>
        /// <param name="sceneName"> 场景名 </param>
        /// <param name="reactTime"> 加载完毕后的延迟时间 </param>
        /// <param name="transitionSceneName"> 加载时的过渡场景名（使用非异步加载，为空时不使用过渡） </param>
        public static void LoadScene(string sceneName, float reactTime = 0f, string transitionSceneName = null)
        {
            if (IsLoading)
            {
#if UNITY_EDITOR
                Debug.LogWarning("已在异步加载场景");
#endif
                return;
            }
            ManagerProxy.Instance.StartCoroutine(LoadSceneCo(sceneName, reactTime, transitionSceneName));
        }
        
        /// <summary> 异步加载目标场景实现 </summary>
        private static IEnumerator LoadSceneCo(string sceneName, float reactTime, string transitionSceneName)
        {
            IsLoading = true;
            if (transitionSceneName != null)
            {
                // 进入过渡场景
                SceneManager.LoadScene(transitionSceneName);
            }
            
            // 启动预加载
            PreLoad(sceneName);
            // 启动异步预加载（如有）
            if (_preLoadReady.Count > 0)
            {
                PreLoadAsync(sceneName);
                while (PreLoadFinished() == false)
                {
                    yield return null;
                }
            }

            // 进度为0
            ProgressUpdate(0.0f);

            // 启动异步加载
            AsyncOperation info = SceneManager.LoadSceneAsync(sceneName);
            info.allowSceneActivation = false;
        
            // 更新进度条
            while (info.progress < 0.9f)
            {
                ProgressUpdate(info.progress);
                yield return null;
            }

            ProgressUpdate(info.progress);
            // 执行预处理（在预加载完成后）
            PreProc(sceneName);
            
            // 执行异步预处理
            if (_preProcReady.Count > 0)
            {
                PreProcAsync(sceneName);
                while (PreProcFinished() == false)
                {
                    yield return null;
                }
            }
            // 加载完毕
            ProgressUpdate(1.0f);

            yield return Wait.Seconds(reactTime);

            // 进入目标场景
            info.allowSceneActivation = true;

            IsLoading = false;
        }
    }
}
