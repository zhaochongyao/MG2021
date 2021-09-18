using System.IO;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Json存储系统
    /// </summary>
    public static class SaveSystem
    {
        /// <summary> 存放的文件夹 </summary>
        private static string _saveFolder;
        
        /// <summary> 初始化 </summary>
        internal static void Init(string folder)
        {
            _saveFolder = folder;
        }
        
        /// <summary> 保存对象 </summary>
        /// <param name="fileName"> 文件名 </param>
        /// <param name="target"> 目标对象 </param>
        public static void Save(string fileName, object target)
        {
            string json = JsonUtility.ToJson(target);

            StreamWriter sw = new StreamWriter(_saveFolder + "/" + fileName + ".json");
            sw.Write(json);
            sw.Close();
        }

        /// <summary> 读取对象 </summary>
        /// <param name="fileName"> 文件名 </param>
        /// <param name="target"> 覆盖目标对象 </param>
        public static void Load(string fileName, object target)
        {
#if UNITY_EDITOR
            if (File.Exists(_saveFolder + "/" + fileName + ".json") == false)
            {
                Debug.LogWarning("No path exists");
                return;
            }
#endif
            StreamReader sr = new StreamReader(_saveFolder + "/" + fileName + ".json");
            string json = sr.ReadToEnd();
            sr.Close();
            JsonUtility.FromJsonOverwrite(json, target);
        }
    }
}