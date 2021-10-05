using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities.DesignPatterns;

namespace KeywordSystem
{
    /// <summary>
    /// 关键词收集器
    /// </summary>
    public class KeywordCollector : LSingleton<KeywordCollector>
    {
        private HashSet<string> _keywordSet;

        /// <summary> 关键词收集事件 </summary>
        public event Action<string> KeywordCollect = delegate { };

        protected override void Awake()
        {
            base.Awake();

            _keywordSet = new HashSet<string>();
        }

        /// <summary>
        /// 收集新的关键词
        /// </summary>
        /// <param name="keyword"> 关键词 </param>
        public void Collect(string keyword)
        {
#if UNITY_EDITOR
            if (_keywordSet.Contains(keyword))
            {
                Debug.LogError("关键词已经收集过了");
                return;
            }
#endif
            _keywordSet.Add(keyword);
            KeywordCollect.Invoke(keyword);
        }

        /// <summary>
        /// 查看关键词是否被收集
        /// </summary>
        /// <param name="keyword"> 关键词 </param>
        public bool Check(string keyword)
        {
            return _keywordSet.Contains(keyword);
        }
    }
}