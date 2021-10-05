using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities.DesignPatterns;

namespace KeywordSystem
{
    public class KeywordCollector : LSingleton<KeywordCollector>
    {
        private HashSet<string> _keywordSet;

        public event Action<string> KeywordCollect = delegate { };

        protected override void Awake()
        {
            base.Awake();

            _keywordSet = new HashSet<string>();
        }

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

        public bool Check(string keyword)
        {
            return _keywordSet.Contains(keyword);
        }
    }
}