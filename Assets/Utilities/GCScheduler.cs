using System;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// 垃圾回收管理
    /// </summary>
    internal static class GCScheduler
    {
        /// <summary> 是否在垃圾回收 </summary>
        private static bool _cleaning;
        
        /// <summary> 初始化 </summary>
        internal static void Init()
        {
            _cleaning = false;
            // 场景加载处于预处理阶段时，手动垃圾回收
            SceneLoader.PreProc += scene => ManagerProxy.Instance.StartCoroutine(CollectGarbageCo());
            SceneLoader.PreProcReadyConnect(() => _cleaning == false);
            // 编辑器模式无法更改垃圾回收模式
#if UNITY_EDITOR == false
            UnityEngine.Scripting.GarbageCollector.GCMode =
                UnityEngine.Scripting.GarbageCollector.Mode.Enabled;
#endif
        }

        private static IEnumerator CollectGarbageCo()
        {

            _cleaning = true;

            /*
            bug： 进度为0
            AsyncOperation ao = Resources.UnloadUnusedAssets();
            // 先回收Unity的，在回收Mono的
            // Unity中无用资源的引用会导致Mono中资源无法回收
            while (ao.isDone == false)
            {
                yield return null;
            }
            */
            GC.Collect();
            yield return null;
            _cleaning = false;

        }
    }
}
