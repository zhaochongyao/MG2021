using System;
using System.Collections.Generic;
using System.Text;
using Singletons;
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

        private KeywordConfigSO _keywordConfigSO;
        private Dictionary<string, string> _mergeKeywordMap;

        protected void Start()
        {
            _keywordSet = new HashSet<string>();
            _keywordConfigSO = GameConfigProxy.Instance.KeywordConfigSO;

            _mergeKeywordMap = new Dictionary<string, string>();
            StringBuilder sb = new StringBuilder();
            foreach (MergeOnlyKeyword merge in _keywordConfigSO.KeywordListSO.MergeOnlyKeywordList)
            {
                sb.Clear();
                merge.Dependency.Sort();
                foreach (string dependencyKeyword in merge.Dependency)
                {
                    sb.Append(dependencyKeyword);
                }
                _mergeKeywordMap.Add(sb.ToString(), merge.Keyword);
            }            
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

        /// <summary>
        /// 关键词合并检查
        /// </summary>
        /// <param name="dependency"> 合并关键词 </param>
        /// <param name="result"> 合并结果 </param>
        public bool MergeCheck(List<string> dependency, out string result)
        {
            StringBuilder sb = new StringBuilder();
            dependency.Sort();
            foreach (string dependencyKeyword in dependency)
            {
                sb.Append(dependencyKeyword);
            }

            return _mergeKeywordMap.TryGetValue(sb.ToString(), out result);
        }
    }
}