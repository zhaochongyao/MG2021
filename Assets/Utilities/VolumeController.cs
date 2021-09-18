using UnityEngine;
using UnityEngine.Audio;

namespace Utilities
{
    /// <summary>
    /// 音量控制器
    /// </summary>
    public static class VolumeController
    {
        /// <summary> 误差 </summary>
        private const float Tolerance = 0.000001f;

        /// <summary> 浮点判等 </summary>
        private static bool Near(float lhs, float rhs)
        {
            return Mathf.Abs(lhs - rhs) < Tolerance;
        }

        /// <summary> 音量下限 </summary>
        private const float LowerLimit = 0.0001f; // Log10(0.0001) * 20 = -4 * 20 = -80

        /// <summary> 音量下限 </summary>
        private const float UpperLimit = 10f; // Log10(10) * 20 = 1 * 20 = 20 
        
        /// <summary> 混音器 </summary>
        private static AudioMixer _audioMixer;

        /// <summary> 静音 </summary>
        private static bool _mute;

        /// <summary> 静音 </summary>
        public static bool Mute
        {
            get => _mute;
            set
            {
                _mute = value;
                if (_mute)
                {
                    _recordMaster = MasterVolume;
                    MasterVolume = 0f;
                }
                else
                {
                    MasterVolume = _recordMaster;
                }
            }
        }

        /// <summary> 主音量记录 </summary>
        private static float _recordMaster;
        
        /// <summary> 主音量 </summary>
        public static float MasterVolume
        {
            get
            {
                _audioMixer.GetFloat("Master", out float value);
                float volume = Mathf.Pow(10f, value / 20f);
                return Near(volume, LowerLimit) ? 0f : volume;
            }
            set
            {
#if UNITY_EDITOR
                if (value < 0f || value > UpperLimit)
                {
                    Debug.LogWarningFormat("非法音量，不在取值范围[0, {0}]内", UpperLimit);
                    return;
                }
#endif
                if (0f <= value && value < LowerLimit)
                {
                    value = LowerLimit;
                }
                else if (_mute)
                {
                    _mute = false;
                }
                _audioMixer.SetFloat("Master", Mathf.Log10(value) * 20f);
            }
        }

        /// <summary> 音效音量 </summary>
        public static float SFXVolume
        {
            get
            {
                _audioMixer.GetFloat("SFX", out float value);
                float volume = Mathf.Pow(10f, value / 20f);
                return Near(volume, LowerLimit) ? 0f : volume;
            }
            set
            {
#if UNITY_EDITOR
                if (value < 0f || value > UpperLimit)
                {
                    Debug.LogWarningFormat("非法音量，不在取值范围[0, {0}]内", UpperLimit);
                    return;
                }
#endif
                if (0f <= value && value < LowerLimit)
                {
                    value = LowerLimit;
                }
                _audioMixer.SetFloat("SFX", Mathf.Log10(value) * 20f);
            }
        }

        /// <summary> 音乐音量 </summary>
        public static float MusicVolume
        {
            get
            {
                _audioMixer.GetFloat("Music", out float value);
                float volume = Mathf.Pow(10f, value / 20f);
                return Near(volume, LowerLimit) ? 0f : volume;
            }
            set
            {
#if UNITY_EDITOR
                if (value < 0f || value > UpperLimit)
                {
                    Debug.LogWarningFormat("非法音量，不在取值范围[0, {0}]内", UpperLimit);
                    return;
                }
#endif
                if (0f <= value && value < LowerLimit)
                {
                    value = LowerLimit;
                }
                _audioMixer.SetFloat("Music", Mathf.Log10(value) * 20f);
            }
        }

        /// <summary> 初始化 </summary>
        internal static void Init(AudioMixer audioMixer)
        {
            _audioMixer = audioMixer;
            OptionSaver.SaveEvent += OnSave;
            OptionSaver.LoadEvent += OnLoad;
            _mute = false;
            _recordMaster = 0f;
        }
        
        /// <summary> 保存 </summary>
        private static void OnSave()
        {
            PlayerPrefs.SetInt("Mute", Mute ? 1 : 0);
            PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
            PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
            PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        }

        /// <summary> 载入 </summary>
        private static void OnLoad()
        {
            Mute = PlayerPrefs.GetInt("Mute") == 1;
            MasterVolume = PlayerPrefs.GetFloat("MasterVolume");
            SFXVolume = PlayerPrefs.GetFloat("SFXVolume");
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        }
    }
}