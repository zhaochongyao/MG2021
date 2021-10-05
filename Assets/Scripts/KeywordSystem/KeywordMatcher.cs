using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Singletons;
using Utilities.DesignPatterns;

namespace KeywordSystem
{
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

        public void SetKeywordList(KeywordListSO keywordListSO)
        {
            _keywordConfigSO.SetKeywordListSO(keywordListSO);
            _acAutomaton.Construct(_keywordConfigSO.KeywordListSO.KeywordList);
        }

        public List<Range> Match(string text)
        {
            return _acAutomaton.Match(text);
        }
    }
}