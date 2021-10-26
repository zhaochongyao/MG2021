// using UnityEngine;
// using UnityEngine.Rendering.PostProcessing;
// using UnityEngine.SceneManagement;
//
// namespace Utilities
// {
//     /// <summary>
//     /// 后处理管理器
//     /// <para> 【支持】辉光、抗锯齿、运动模糊、景深、环境光遮蔽 </para>
//     /// </summary>
//     public static class PostProcessManager
//     {
//         /// <summary> 后处理区域 </summary>
//         private static PostProcessVolume _postProcessVolume;
//
//         /// <summary> 后处理层 </summary>
//         private static PostProcessLayer _postProcessLayer;
//             
//         // 从Packages内的后处理模块拖入
//         /// <summary> 后处理资源 </summary>
//         private static PostProcessResources _postProcessResources;
//
//         /// <summary> 抗锯齿 </summary>
//         public static PostProcessLayer.Antialiasing Antialiasing
//         {
//             get => _postProcessLayer.antialiasingMode;
//             set => _postProcessLayer.antialiasingMode = value;
//         }
//
//         /// <summary> SMAA抗锯齿 </summary>
//         public static SubpixelMorphologicalAntialiasing SMAA => _postProcessLayer.subpixelMorphologicalAntialiasing;
//
//         /// <summary> FXAA抗锯齿 </summary>
//         public static FastApproximateAntialiasing FXAA => _postProcessLayer.fastApproximateAntialiasing;
//
//         /// <summary> TAA抗锯齿 </summary>
//         public static TemporalAntialiasing TAA => _postProcessLayer.temporalAntialiasing;
//
//         /// <summary> 辉光 </summary>
//         private static Bloom _bloom;
//
//         /// <summary> 辉光 </summary>
//         public static Bloom BloomFX => _bloom;
//
//         /// <summary> 运动模糊 </summary>
//         private static MotionBlur _motionBlur;
//
//         /// <summary> 运动模糊 </summary>
//         public static MotionBlur MotionBlurFX => _motionBlur;
//
//         /// <summary> 景深 </summary>
//         private static DepthOfField _depthOfField;
//
//         /// <summary> 景深 </summary>
//         public static DepthOfField DepthOfFieldFX => _depthOfField;
//
//         /// <summary> 环境光遮蔽 </summary>
//         private static AmbientOcclusion _ambientOcclusion;
//
//         /// <summary> 环境光遮蔽 </summary>
//         public static AmbientOcclusion AmbientOcclusionFX => _ambientOcclusion;
//
//         /// <summary> 启用后处理 </summary>
//         public static bool PostProcessOn
//         {
//             get => _postProcessVolume.enabled;
//             set => _postProcessVolume.enabled = value;
//         }
//
//         /// <summary> 初始化 </summary>
//         internal static void Init(GameObject postProcessGameObject, PostProcessResources postProcessResources)
//         {
//             _postProcessResources = postProcessResources;
//             // 后处理
//             UnityEngine.Object.DontDestroyOnLoad(postProcessGameObject);
//             _postProcessVolume = postProcessGameObject.GetComponent<PostProcessVolume>();
//             _postProcessVolume.profile.TryGetSettings(out _bloom);
//             _postProcessVolume.profile.TryGetSettings(out _motionBlur);
//             _postProcessVolume.profile.TryGetSettings(out _depthOfField); 
//             _postProcessVolume.profile.TryGetSettings(out _ambientOcclusion);
//             // 初始关闭后处理特效
//             _postProcessVolume.enabled = false;
//             
//             // 订阅事件
//             SceneLoader.ActiveSceneChanged += OnSceneChanged;
//             _firstTime = true;
//         }
//         
//         /// <summary> 第一次初始化 </summary>
//         private static bool _firstTime;
//         
//         /// <summary> 抗锯齿选项 </summary>
//         private static PostProcessLayer.Antialiasing _preAntialiasing;
//
//         /// <summary> FXAA记录 </summary>
//         private static FastApproximateAntialiasing _fxaa;
//         
//         /// <summary> SMAA记录 </summary>
//         private static SubpixelMorphologicalAntialiasing _smaa;
//         
//         /// <summary> TAA记录 </summary>
//         private static TemporalAntialiasing _taa;
//         
//         /// <summary> 场景切换时 </summary>
//         private static void OnSceneChanged(Scene before, Scene after)
//         {
//             // 新场景无摄像机、不操作
//             if (Camera.main == null)
//             {
//                 return;
//             }
//             // 给摄像机添加Layer组件
//             _postProcessLayer = Camera.main.gameObject.AddComponent<PostProcessLayer>();
//             _postProcessLayer.volumeLayer = (1 << LayerMask.NameToLayer("PostProcess"));
//             
//             // 第一次初始化
//             if (_firstTime)
//             {
//                 // 抗锯齿默认使用最高质量的SMAA
//                 _postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
//                 _postProcessLayer.subpixelMorphologicalAntialiasing.quality = 
//                     SubpixelMorphologicalAntialiasing.Quality.High;
//                 _firstTime = false;
//             }
//             else
//             {
//                 // 给新建的Layer初始化资源，脚本添加不会初始化（除第一次）
//                 _postProcessLayer.Init(_postProcessResources);
//                 // 读取记录
//                 _postProcessLayer.antialiasingMode = _preAntialiasing;
//                 _postProcessLayer.fastApproximateAntialiasing = _fxaa;
//                 _postProcessLayer.subpixelMorphologicalAntialiasing = _smaa;
//                 _postProcessLayer.temporalAntialiasing = _taa;
//             }
//             // 记录
//             _preAntialiasing = _postProcessLayer.antialiasingMode;
//             _fxaa = FXAA;
//             _smaa = SMAA;
//             _taa = TAA;
//         }
//     }
// }
