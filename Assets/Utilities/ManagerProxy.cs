using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using Utilities.DataStructures;

namespace Utilities
{
    /// <summary>
    /// 静态管理类的单例代理
    /// 使用SerializeField帮助管理类获取引用
    /// 同时作为静态管理类（非MonoBehaviour子类）的协程启动器
    /// </summary>
    internal class ManagerProxy : MonoBehaviour
    {
        /// <summary> 单例 </summary>
        internal static ManagerProxy Instance { get; private set; }

        /// <summary> 场景对应对象池配置 </summary>
        [Header("[PoolConfigurator]")] [SerializeField]
        private SerializableDictionary<string, PoolSettingSO> _poolSettings;

        /// <summary> 混音器 </summary>
        [Header("[VolumeController]")] [SerializeField]
        private AudioMixer _audioMixer;

        /// <summary> 后处理配置游戏对象 </summary>
        [Header("[PostProcessManager]")] [SerializeField]
        private GameObject _postProcessGameObject;

        /// <summary> 后处理资源 </summary>
        // 从Packages内的后处理模块拖入
        [SerializeField] private PostProcessResources _postProcessResources;

        internal event Action UpdateEvent;

        internal event Action FixedUpdateEvent;

        /// <summary> 单例初始化 </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                // 初始化
                Init();
                // 待全部初始化后，执行初始操作
                Begin();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary> 初始化 </summary>
        private void Init()
        {
            SceneLoader.Init();
            OptionSaver.Init();
            // 需要初始化事件的静态类必须在按依赖关系放在初始化前排

            GCScheduler.Init();
            GraphicOptions.Init();
            PoolConfigurator.Init(_poolSettings);
            VolumeController.Init(_audioMixer);
            PostProcessManager.Init(_postProcessGameObject, _postProcessResources);

            SaveSystem.Init(Application.persistentDataPath);
            
            UpdateEvent = delegate {  };
            FixedUpdateEvent = delegate {  };
        }

        private void Begin()
        {
            OptionSaver.Begin();
        }

        private void Update()
        {
            UpdateEvent.Invoke();
        }

        private void FixedUpdate()
        {
            FixedUpdateEvent.Invoke();
        }
    }
}