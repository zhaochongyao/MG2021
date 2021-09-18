using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// 对象池配置器
    /// </summary>
    internal static class PoolConfigurator
    {
        /// <summary> 场景与其对象池设置的映射集合 </summary>
        private static IDictionary<string, PoolSettingSO> _poolSettings;

        /// <summary> 正在调整对象池 </summary>
        private static bool _adjusting;

        /// <summary> 初始化 </summary>
        internal static void Init(IDictionary<string, PoolSettingSO> poolSettings)
        {
            _poolSettings = poolSettings;

            _adjusting = false;
            SceneLoader.PreLoadAsync += OnPreLoad;
            SceneLoader.PreLoadReadyConnect(() => _adjusting == false);
        }
        
        /// <summary> 场景异步加载时，在预加载阶段调整对象池配置 </summary>
        private static void OnPreLoad(string sceneName)
        {
#if UNITY_EDITOR
            if (_poolSettings.ContainsKey(sceneName) == false)
            {
                Debug.LogWarning("不存在该场景");
                return;
            }
#endif
            ManagerProxy.Instance.StartCoroutine(PreparePoolCo(sceneName));
        }

        /// <summary> 准备对象池 </summary>
        private static IEnumerator PreparePoolCo(string sceneName)
        {
            _adjusting = true;
            HashSet<AsyncHandle> handles = new HashSet<AsyncHandle>();
            // 一次性启动所有对象池的异步Resize
            foreach (KeyValuePair<GameObject, int> pair in _poolSettings[sceneName]._poolSettingItems)
            {
                AsyncHandle ah = ObjectPool.ResizeAsync(pair.Key, pair.Value);
                handles.Add(ah);
            }

            while (handles.Count > 0)
            {
                // 每帧找出已经完成的异步操作
                // 移出集合
                // 不能在foreach中直接Remove集合元素
                handles.RemoveWhere(ah => ah.IsDone);
                yield return null;
            }
            _adjusting = false;
        }
    }
}