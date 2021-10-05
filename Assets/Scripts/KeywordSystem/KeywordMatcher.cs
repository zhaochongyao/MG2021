using System.Collections.Generic;
using Singletons;
using Utilities.DesignPatterns;

namespace KeywordSystem
{
    /// <summary>
    /// 关键词匹配器
    /// </summary>
    public class KeywordMatcher : LSingleton<KeywordMatcher>
    {
        private AcAutomaton _acAutomaton;
        private KeywordConfigSO _keywordConfigSO;

        private void Start()
        {
            _acAutomaton = new AcAutomaton();
            _keywordConfigSO = GameConfigProxy.Instance.KeywordConfigSO;
            if (_keywordConfigSO.KeywordListSO != null)
            {
                _acAutomaton.Construct(_keywordConfigSO.KeywordListSO.KeywordList);
            }
        }

        /// <summary>
        /// 设置新的关键词列表
        /// </summary>
        /// <param name="keywordListSO"> 关键词列表 </param>
        public void SetKeywordList(KeywordListSO keywordListSO)
        {
            _keywordConfigSO.SetKeywordListSO(keywordListSO);
            _acAutomaton.Construct(_keywordConfigSO.KeywordListSO.KeywordList);
        }

        /// <summary>
        /// 匹配一段文字，找出关键词
        /// </summary>
        /// <param name="text"> 目标文字 </param>
        /// <returns> 关键词下标区间 </returns>
        public List<Range> Match(string text)
        {
            return _acAutomaton.Match(text);
        }
    }
}