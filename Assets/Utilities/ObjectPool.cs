using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.DataStructures;

namespace Utilities
{
    /// <summary>
    /// 对象池
    ///【使用指导】
    /// <para> （1）对象的重置操作写在OnEnable中，
    /// 只想进行一次的初始化操作写在Awake中（Start不会执行）； </para>
    /// 
    /// <para> （2）当异步加载未完成时也可进行获取，数量不足将再额外多申请一个（无论是否在异步加载）；
    /// 等待对象池异步加载结束，再进行获取 </para>
    ///
    /// <para> （3）对象池模块的运行与预制体的命名相关，若运行时遭到修改，可能无法正常运行；</para>
    ///
    /// </summary>
    public sealed class ObjectPool : MonoBehaviour
    {
        /// <summary> 对象预制体 </summary>
        [SerializeField] private GameObject _prefab;

        /// <summary> 物体默认位置 </summary>
        private Vector3 _defaultPosition;

        /// <summary> 物体默认旋转 </summary>
        private Quaternion _defaultRotation;

        /// <summary> 池预设大小 </summary>
        [Min(0), SerializeField] private int _defaultSize;

        /// <summary> 正在执行异步操作 </summary>
        private bool _inAsyncOperation;

        /// <summary>
        /// 预热模式
        /// </summary>
        public enum PreWarmMode
        {
            Sync,  // 同步
            Async, // 异步
            Dynamic  // 动态
        }
        
        /// <summary> 预热模式 </summary>
        [SerializeField] private PreWarmMode _preWarmMode;
        
        /// <summary> 默认异步帧间隔 </summary>
        [Min(1), SerializeField] private int _defaultWaitFrames = 1;
        
        /// <summary> 默认每次异步间隔的加载/卸载个数 </summary>
        [Min(1) ,SerializeField] private int _defaultObjectsPerWait = 5;


        /// <summary> 异步操作完成事件 </summary>
        public event Action AsyncDoneEvent;

        /// <summary> 父结点缓存 </summary>
        private Transform _objectsParentNode;

        /// <summary> 当前可用的物体 </summary>
        private Stack<GameObject> _currentObjects;

        /// <summary> 全部申请的物体 </summary>
        private Stack<GameObject> _appliedObjects;

        /// <summary> 当前归还的物体 </summary>
        private Stack<GameObject> _returningObjects;

        /// <summary> 对象池字典 </summary>
        private static readonly Dictionary<string, ObjectPool> _poolsMapping = 
            new Dictionary<string, ObjectPool>();

        /// <summary> 管理器结点 </summary>
        private static Transform _poolManagerNode;

        /// <summary> 初始化 </summary>
        private void Awake()
        {
            if (_poolsMapping.ContainsKey(_prefab.name))
            {
                // 同一对象池，放在多个场景中时，只保留最初的
                Destroy(gameObject);
                return;
            }
            
            _objectsParentNode = transform;
            if (_poolManagerNode == null)
            {
                // 所有对象池父节点
                GameObject go = 
                    transform.parent != null ? 
                    _objectsParentNode.parent.gameObject :
                    new GameObject();
                go.name = "ObjectPool";
                DontDestroyOnLoad(go);
                _poolManagerNode = go.transform;
            }
            
            // 托管
            _objectsParentNode.SetParent(_poolManagerNode);
            _poolsMapping.Add(_prefab.name, this);
            _objectsParentNode.gameObject.name = _prefab.name + "Pool";
            
            // 初始化
            _defaultPosition = _prefab.transform.position;
            _defaultRotation = _prefab.transform.rotation;

            _currentObjects = new Stack<GameObject>(_defaultSize);
            _appliedObjects = new Stack<GameObject>(_defaultSize);
            _returningObjects = new Stack<GameObject>();
            _inAsyncOperation = false;
            // 获取物体最早只能在Start或OnEnable内进行，以防预热还未完成

            // 对象的Prefab必须设置Enable为false，
            // 在初次加载不会执行Awake和OnEnable，浪费资源
            _prefab.SetActive(false);
            
            // 开启预热
            if (_preWarmMode == PreWarmMode.Async)
            {
                BaseResizeAsync(_defaultSize, _defaultWaitFrames, _defaultObjectsPerWait);
            }
            else if (_preWarmMode == PreWarmMode.Sync)
            {
                BaseResize(_defaultSize);
            }
            // 动态模式不加载对象
        }

        /// <summary> 游戏结束时，将prefab设为true </summary>
        private void OnDestroy()
        {
            _prefab.SetActive(true);
        }

        /// <summary> 申请单个对象 </summary>
        private GameObject Apply()
        {
            // 对象实例设置为其子物体
            GameObject go = Instantiate(_prefab, _defaultPosition, _defaultRotation, _objectsParentNode);
            // 初始世界位置保持不变，与prefab默认位置相同，附属到对象池物体下管理
            go.name = _prefab.name;
            _appliedObjects.Push(go);
            return go;
        }

        /// <summary> 调整大小 </summary>
        private void BaseResize(int newSize)
        {
            // Extend
            while (_appliedObjects.Count < newSize)
            {
                GameObject go = Apply();
                _currentObjects.Push(go);
            }
            // Shrink
            while (_appliedObjects.Count > newSize)
            {
                Destroy(_appliedObjects.Pop());
            }
        }

        /// <summary> 协程调整大小 </summary>
        private IEnumerator BaseResizeCo(int newSize, int asyncFrames, int objectsPerSlice)
        {
            _inAsyncOperation = true;
           
            while (_appliedObjects.Count < newSize)
            {
                for (int i = 1; i <= objectsPerSlice && _appliedObjects.Count < newSize; ++i)
                {
                    GameObject go = Apply();
                    _currentObjects.Push(go);              
                }
                yield return WaitCache.Frames(asyncFrames);
            }

            while (_appliedObjects.Count > newSize)
            {
                for (int i = 1; i <= objectsPerSlice && _appliedObjects.Count > newSize; ++i)
                {
                    Destroy(_appliedObjects.Pop());
                }

                yield return WaitCache.Frames(asyncFrames);
            }

            System.Diagnostics.Debug.Assert(AsyncDoneEvent != null, nameof(AsyncDoneEvent) + " != null");
            AsyncDoneEvent.Invoke();
            _inAsyncOperation = false;
        }

        /// <summary> 异步调整大小 </summary>
        private AsyncHandle BaseResizeAsync(int newSize, int waitFrames, int objectsPerWait)
        {
#if UNITY_EDITOR
            if (_inAsyncOperation)
            {
                Debug.LogWarning("已有异步操作执行，请等待其完成后再进行异步操作请求");
                return null;
            }
#endif
            AsyncHandle ah = new AsyncHandle(this);
            StartCoroutine(BaseResizeCo(newSize, waitFrames, objectsPerWait));
            return ah;
        }

        /// <summary> 获取物体 </summary>
        private GameObject BaseSpawn()
        {
            // 默认申请完的物体或已归还池内的物体位置旋转已重设为默认
            
            // 不足，申请新的
            // 即便正在异步加载，也重新申请新的
            return _currentObjects.Count == 0 ? Apply() : _currentObjects.Pop();

            // 可在对象的OnEnable中重设对象参数
            // 这意味着一次性的初始化操作（非重设操作）
            // 应当在Awake内完成而非Start
        }

        /// <summary> 延迟返还物体实现 </summary>
        private IEnumerator BaseReturnCo(GameObject go, float delay)
        {
            yield return WaitCache.Seconds(delay);
            // 加入缓冲栈
            _returningObjects.Push(go);
        }

        /// <summary> 延迟返还物体 </summary>
        private void BaseReturn(GameObject go, float delay)
        {
            if (delay == 0)
            {
                _returningObjects.Push(go);
            }
            else
            {
                StartCoroutine(BaseReturnCo(go, delay));
            }
        }

        /// <summary> 在Update结束后，渲染前执行真正的Return操作 </summary>
        private void LateUpdate()
        {
            // 清空缓冲栈
            while (_returningObjects.Count > 0)
            {
                GameObject go = _returningObjects.Pop();
                go.SetActive(false);
                // 恢复原位置和旋转
                go.transform.SetPositionAndRotation(_defaultPosition, _defaultRotation);

                // SetParent时默认保持原世界位置
                // 当显式将worldPositionStays设为false时
                // 物体位置为父位置+物体默认位置
                go.transform.SetParent(_objectsParentNode);
                // SetParent(_father) == SetParent(_father, true);
                // (worldPositionStays == true)

                _currentObjects.Push(go);
            }
        }

        /// <summary> 获取物体 </summary>
        /// <param name="original"> 预制体名 </param>
        public static GameObject Spawn(GameObject original)
        {
            GameObject go = _poolsMapping[original.name].BaseSpawn();
            go.SetActive(true);
            return go;
        }

        /// <summary> 获取物体并设定父物体（世界位置为默认位置+父物体位置） </summary>
        /// <param name="original"> 预制体名 </param>
        /// <param name="parent"> 父物体 </param>
        /// <param name="spawnInWorldSpace"> ，为true则为默认位置，为false物体位置为父物体位置+默认位置 </param>
        public static GameObject Spawn(GameObject original, Transform parent, bool spawnInWorldSpace = false)
        {
            GameObject go = _poolsMapping[original.name].BaseSpawn();
            go.transform.SetParent(parent, spawnInWorldSpace);
            // SetActive放在最后，此时执行OnEnable，保证对象已经在新的parent下，而不是对象池结点
            go.SetActive(true);
            return go;
        }

        /// <summary> 获取物体并设定位置和旋转 </summary>
        /// <param name="original"> 预制体 </param>
        /// <param name="position"> 位置 </param>
        /// <param name="rotation"> 旋转 </param>
        public static GameObject Spawn(GameObject original, Vector3 position, Quaternion rotation)
        {
            GameObject go = _poolsMapping[original.name].BaseSpawn();
            go.transform.SetPositionAndRotation(position, rotation);
            go.SetActive(true);
            return go;
        }
        
        /// <summary> 获取物体并设定位置，旋转和父物体（位置为世界坐标，不与父物体相关） </summary>
        /// <param name="original"> 预制体 </param>
        /// <param name="position"> 位置 </param>
        /// <param name="rotation"> 旋转 </param>
        /// <param name="parent"> 父物体 </param>
        public static GameObject Spawn(GameObject original, Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject go = _poolsMapping[original.name].BaseSpawn();
            go.transform.SetPositionAndRotation(position, rotation);
            go.transform.SetParent(parent);
            go.transform.localScale = Vector3.one;
            go.SetActive(true);
            return go;
        }

        /// <summary> 延迟返还物体 </summary>
        /// <param name="go"> 返还的游戏物体 </param>
        /// <param name="delay"> 返还的延迟时间 </param>
        public static void Return(GameObject go, float delay = 0f)
        {
            _poolsMapping[go.name].BaseReturn(go, delay);
        }

        /// <summary> 调整大小 </summary>
        /// <param name="prefab"> 预制体 </param>
        /// <param name="newSize"> 新大小 </param>
        public static void Resize(GameObject prefab, int newSize)
        {
            _poolsMapping[prefab.name].BaseResize(newSize);
        }

        /// <summary> 异步调整大小 </summary>
        /// <param name="prefab"> 预制体 </param>
        /// <param name="newSize"> 新大小 </param>
        /// <param name="waitFrames"> 等待间隔帧数 </param>
        /// <param name="objectsPerWait"> 每次等待个数 </param>
        /// <returns></returns>
        public static AsyncHandle ResizeAsync(GameObject prefab, int newSize, int waitFrames, int objectsPerWait)
        {
            return _poolsMapping[prefab.name].BaseResizeAsync(newSize, waitFrames, objectsPerWait);
        }

        /// <summary> 异步调整大小 </summary>
        /// <param name="prefab"> 预制体 </param>
        /// <param name="newSize"> 新大小 </param>
        public static AsyncHandle ResizeAsync(GameObject prefab, int newSize)
        {
            int waitFrames = _poolsMapping[prefab.name]._defaultWaitFrames;
            int objectsPerWait = _poolsMapping[prefab.name]._defaultObjectsPerWait;
            return _poolsMapping[prefab.name].BaseResizeAsync(newSize, waitFrames, objectsPerWait);
        }
    }
}
