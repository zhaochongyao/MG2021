using System.Collections.Generic;
using UnityEngine;

namespace KeywordSystem
{
    [CreateAssetMenu(fileName = "KeywordList", menuName = "Keyword Component/Keyword List")]
    public class KeywordListSO : ScriptableObject
    {
        [SerializeField] private List<string> _keywordList;

        public List<string> KeywordList => _keywordList;
    }
}