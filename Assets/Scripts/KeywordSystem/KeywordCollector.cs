using System.Collections.Generic;
using Utilities.DesignPatterns;

namespace KeywordSystem
{
    public class KeywordCollector : LSingleton<KeywordCollector>
    {
        private HashSet<string> _keywordSet;
        
        protected override void Awake()
        {
            base.Awake();

            _keywordSet = new HashSet<string>();
        }

        public void Collect(string keyword)
        {
            _keywordSet.Add(keyword);
        }

        public bool Check(string keyword)
        {
            return _keywordSet.Contains(keyword);
        }
    }
}