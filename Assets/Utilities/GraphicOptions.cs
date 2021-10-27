using System.Xml.Linq;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// 图形设置
    /// <para> 【注意】帧率设置只在垂直同步关闭时有效！
    /// Edit/ProjectSetting/QualitySettings/VSyncCount
    /// 或代码控制也可以 </para>
    ///
    /// <para> 【支持】分辨率、全屏、控制帧率、获取帧率、垂直同步 </para>
    /// </summary>
    public static class GraphicOptions
    {
        /// <summary> 宽度 </summary>
        public static int Width => Screen.width;

        /// <summary> 高度 </summary>
        public static int Height => Screen.height;
        
        /// <summary> 设置宽高 </summary>
        public static void SetWidthHeight(int width, int height)
        {
            Screen.SetResolution(width, height, FullScreen);
        }

        // 帧率设置为-1时，不限制帧率
        // 帧率设置只在垂直同步关闭时有效！
        /// <summary> 目标帧率 </summary>
        public static int TargetFrameRate
        {
            get => Application.targetFrameRate;
            set => Application.targetFrameRate = value;
        }

        /// <summary> 当前帧率 </summary>
        public static int CurrentFrameRate => (int) (1.0f / Time.deltaTime);

        /// <summary> 全屏 </summary>
        public static bool FullScreen
        {
            get => Screen.fullScreen;
            set => Screen.fullScreen = value;
        }

        /// <summary> 垂直同步 </summary>
        private static bool _vsync;

        /// <summary> 垂直同步 </summary>
        public static bool VSync
        {
            get => _vsync;
            set
            {
                _vsync = value;
                QualitySettings.vSyncCount = _vsync ? 1 : 0;
                // 1代表有垂直同步
                // 0代表无垂直同步
            }
        }
        
        /// <summary> 初始化 </summary>
        internal static void Init()
        {
            // OptionSaver.SaveEvent += OnSave;
            // OptionSaver.LoadEvent += OnLoad;
            // 初始不限制帧率
            Application.targetFrameRate = -1;
            // 垂直同步默认不开
            _vsync = false;
            QualitySettings.vSyncCount = 0;
        }

        // /// <summary> 保存 </summary>
        // private static void OnSave()
        // {
        //     PlayerPrefs.SetInt("Width", Width);
        //     PlayerPrefs.SetInt("Height", Height);
        //     PlayerPrefs.SetInt("TargetFrameRate", TargetFrameRate);
        //     PlayerPrefs.SetInt("FullScreen", FullScreen ? 1 : 0);
        //     PlayerPrefs.SetInt("VSync", VSync ? 1 : 0);
        // }
        //
        // /// <summary> 载入 </summary>
        // private static void OnLoad()
        // {
        //     int width = PlayerPrefs.GetInt("Width");
        //     int height = PlayerPrefs.GetInt("Height");
        //     SetWidthHeight(width, height);
        //     FullScreen = PlayerPrefs.GetInt("FullScreen") == 1;
        //     TargetFrameRate = PlayerPrefs.GetInt("TargetFrameRate");
        //     VSync = PlayerPrefs.GetInt("VSync") == 1;
        // }
    }
}
