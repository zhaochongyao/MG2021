using System.Collections.Generic;
using UnityEngine;

namespace KeywordSystem
{
    [System.Serializable]
    public struct MergeOnlyKeyword
    {
        [SerializeField] private string _keyword;
        [SerializeField] private List<string> _dependency;

        public string Keyword => _keyword;
        public List<string> Dependency => _dependency;
    }
    
    [CreateAssetMenu(fileName = "KeywordList", menuName = "Keyword Component/Keyword List")]
    public class KeywordListSO : ScriptableObject
    {
        [SerializeField] private List<string> _keywordList;
        [SerializeField] private List<MergeOnlyKeyword> _mergeOnlyKeywordList;
        
        public List<string> KeywordList => _keywordList;
        public List<MergeOnlyKeyword> MergeOnlyKeywordList => _mergeOnlyKeywordList;
    }
}