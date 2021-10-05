using UnityEngine;

namespace KeywordSystem
{
    [CreateAssetMenu(fileName = "KeywordConfig", menuName = "Keyword Component/Keyword Config")]
    public class KeywordConfigSO : ScriptableObject
    {
        [SerializeField] private Color _highLightColor = new Color(1f, 1f, 1f, 1f);
        [SerializeField] private Color _collectedColor = new Color(1f, 1f, 1f, 1f);

        [SerializeField] private KeywordListSO _keywordListSO;
        [SerializeField] private GameObject _keywordBackgroundPrefab;

        [SerializeField] private float _clickBoxExpandRatio;
        
        public Color HighLightColor => _highLightColor;
        public Color CollectedColor => _collectedColor;

        public KeywordListSO KeywordListSO => _keywordListSO;
        public GameObject KeywordBackgroundPrefab => _keywordBackgroundPrefab;

        public float ClickBoxExpandRatio => _clickBoxExpandRatio;
        
        public void SetKeywordListSO(KeywordListSO keywordListSO)
        {
            _keywordListSO = keywordListSO;
        }
    }
}