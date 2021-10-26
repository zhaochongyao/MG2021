using Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.DataStructures;

namespace KeywordSystem
{
    public class FakeKeywordButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private string _keywordOverride;
        private KeywordConfigSO _keywordConfigSO;

        private string _keyword;
        
        private void Start()
        {
            _keyword = string.IsNullOrEmpty(_keywordOverride) ? _text.text : _keywordOverride;
            
            _keywordConfigSO = GameConfigProxy.Instance.KeywordConfigSO;
            _text.color = _keywordConfigSO.FakeHighLightColor;

            Wait.Delayed(() =>
            {
                if (KeywordCollector.Instance.Check(_keyword))
                {
                    _text.color = _keywordConfigSO.CollectedColor;
                    return;
                }

                KeywordCollector.Instance.KeywordCollect += OnCollect;
            
                _button.onClick.AddListener(() =>
                {
                    if (KeywordCollector.Instance.Check(_keyword) == false)
                    {
                        KeywordCollector.Instance.Collect(_keyword);
                        _text.color = _keywordConfigSO.CollectedColor;
                    }
                });   
            }, 0.1f);
        }

        private void OnCollect(string keyword)
        {
            if (_keyword == keyword)
            {
                _text.color = _keywordConfigSO.CollectedColor;
            }
        }
    }
}