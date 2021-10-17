using System.Collections;
using System.Collections.Generic;
using KeywordSystem;
using Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.DataStructures;
using Utilities.DesignPatterns;

namespace Iphone
{
    public class DingGuaGua : LSingleton<DingGuaGua>
    {
        [Header("Button")] [SerializeField] private GameObject _workingArea;
        [SerializeField] private GameObject _clientDetail;

        [SerializeField] private GameObject _clientData;
        [SerializeField] private GameObject _memorandum;

        private IphoneConfigSO _iphoneConfigSO;

        [Header("BaseData")] [SerializeField] private Image _baseDataProgressBar;
        private int _collectedData;
        private int _maxData;
        [SerializeField] private TextMeshProUGUI _clientName;

        [SerializeField] private Toggle _maleToggle;
        [SerializeField] private Toggle _femaleToggle;

        [SerializeField] private Slider _ageSlider;
        [SerializeField] private TextMeshProUGUI _ageText;

        [SerializeField] private TextMeshProUGUI _occupationText;
        [SerializeField] private TextMeshProUGUI _educationBackgroundText;

        [Header("ClientBackground")] [SerializeField]
        private Image _clientBackgroundProgressBar;

        private int _collectedBackground;
        private int _maxBackground;
        [SerializeField] private Transform _backgroundLayoutObject;

        [SerializeField] private GameObject _backgroundTextPrefab;
        private TextMeshProUGUI[] _backgroundTexts;

        [Header("Memorandum")] [SerializeField]
        private Image _keywordCollectProgressBar;

        private int _collectedKeyword;
        private int _maxKeyword;

        [SerializeField] private Transform _keywordLayoutGroup;
        [SerializeField] private GameObject _keywordPrefab;

        [SerializeField] private TextMeshProUGUI[] _mergeTryTexts;
        [SerializeField] private TextMeshProUGUI _mergeResult;
        [SerializeField] private GameObject _mergeButton;
        
        private List<string> _mergeTryList;
        private KeywordCollector _keywordCollector;

        [SerializeField] private float _mergeResultShowTime;

        [SerializeField] private GameObject _clientDataRedDot;
        [SerializeField] private GameObject _memorandumRedDot;

        private void Start()
        {
            _iphoneConfigSO = GameConfigProxy.Instance.IphoneConfigSO;

            _baseDataProgressBar.fillAmount = 0f;
            _clientBackgroundProgressBar.fillAmount = 0f;

            _maxData = 5;
            _maxBackground = _iphoneConfigSO.ClientBackgrounds.Length;

            _backgroundTexts = new TextMeshProUGUI[_maxBackground];
            for (int i = 0; i < _backgroundTexts.Length; ++i)
            {
                GameObject go = Instantiate(_backgroundTextPrefab, _backgroundLayoutObject);
                _backgroundTexts[i] = go.GetComponent<TextMeshProUGUI>();
            }

            KeywordListSO keywordListSO = GameConfigProxy.Instance.KeywordConfigSO.KeywordListSO;
            _maxKeyword = keywordListSO.KeywordList.Count + keywordListSO.MergeOnlyKeywordList.Count - _maxData;
            _mergeTryList = new List<string>(4);

            _keywordCollector = KeywordCollector.Instance;
            _keywordCollector.KeywordCollect += OnKeywordCollect;
        }

        private void UpdateDataProgress()
        {
            if (_clientData.activeInHierarchy == false && _clientDataRedDot.activeSelf == false)
            {
                RedDotManager.Instance.ShowRedDot(_clientDataRedDot);
            }
            
            _collectedData++;
            _baseDataProgressBar.fillAmount = (float) _collectedData / _maxData;
        }

        private void UpdateBackgroundProgress()
        {
            if (_clientData.activeInHierarchy == false && _clientDataRedDot.activeSelf == false)
            {
                RedDotManager.Instance.ShowRedDot(_clientDataRedDot);
            }
            
            _collectedBackground++;
            _clientBackgroundProgressBar.fillAmount = (float) _collectedBackground / _maxBackground;
        }

        private void UpdateKeywordCollectProgress()
        {
            if (_memorandum.activeInHierarchy == false && _memorandumRedDot.activeSelf == false)
            {
                RedDotManager.Instance.ShowRedDot(_memorandumRedDot);
            }
            
            _collectedKeyword++;
            _keywordCollectProgressBar.fillAmount = (float) _collectedKeyword / _maxKeyword;
        }

        public void Merge()
        {
            if (_keywordCollector.MergeCheck(_mergeTryList, out string res) 
                && _keywordCollector.Check(res) == false)
            {
                _keywordCollector.Collect(res);

                // 是否为背景
                for (int i = 0; i < _iphoneConfigSO.ClientBackgrounds.Length; ++i)
                {
                    string background = _iphoneConfigSO.ClientBackgrounds[i];
                    if (res == background)
                    {
                        _backgroundTexts[i].text = background;
                        StartCoroutine(ShowMergeResultCo(res));
                        UpdateBackgroundProgress();

                        break;
                    }
                }
            }

            DeleteMergeTry();
        }

        private IEnumerator ShowMergeResultCo(string res)
        {
            Transform mergeResultObject = _mergeResult.transform.parent;
            _mergeResult.text = res;
            _mergeButton.SetActive(false);
            mergeResultObject.gameObject.SetActive(true);
            yield return WaitCache.Seconds(_mergeResultShowTime);
            _mergeButton.SetActive(true);
            mergeResultObject.gameObject.SetActive(false);
        }

        public void DeleteMergeTry()
        {
            _mergeTryList.Clear();
            foreach (TextMeshProUGUI mergeTry in _mergeTryTexts)
            {
                mergeTry.text = null;
            }
        }

        private void OnKeywordCollect(string keyword)
        {
            // 收集客户基础数据
            if (keyword == _iphoneConfigSO.ClientName)
            {
                _clientName.text = keyword;
                UpdateDataProgress();
            }
            else if (keyword == _iphoneConfigSO.SexText)
            {
                Toggle target = _iphoneConfigSO.IsMale ? _maleToggle : _femaleToggle;
                target.isOn = true;
                UpdateDataProgress();
            }
            else if (keyword == _iphoneConfigSO.Age)
            {
                _ageSlider.value = int.Parse(_iphoneConfigSO.Age);
                _ageText.text = _iphoneConfigSO.Age;
                UpdateDataProgress();
            }
            else if (keyword == _iphoneConfigSO.Occupation)
            {
                _occupationText.text = _iphoneConfigSO.Occupation;
                UpdateDataProgress();
            }
            else if (keyword == _iphoneConfigSO.EducationBackground)
            {
                _educationBackgroundText.text = _iphoneConfigSO.EducationBackground;
                UpdateDataProgress();
            }
            // 收集普通关键词
            else
            {
                GameObject go = Instantiate(_keywordPrefab, _keywordLayoutGroup);
                Button btn = go.GetComponent<Button>();
                TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();

                txt.text = keyword;
                btn.onClick.AddListener
                (
                    () =>
                    {
                        foreach (string added in _mergeTryList)
                        {
                            if (keyword == added)
                            {
                                return;
                            }
                        }
                        _mergeTryTexts[_mergeTryList.Count].text = keyword;
                        _mergeTryList.Add(keyword);
                    }
                );
                UpdateKeywordCollectProgress();
            }
        }

        public void ToWorkingArea()
        {
            _workingArea.SetActive(true);
            _clientDetail.SetActive(false);
        }

        public void ToClientDetail()
        {
            if (_clientData.activeSelf && _clientDataRedDot.activeSelf)
            {
                RedDotManager.Instance.HideRedDot(_clientDataRedDot);
            }
            if (_memorandum.activeSelf && _memorandumRedDot.activeSelf)
            {
                RedDotManager.Instance.HideRedDot(_memorandumRedDot);
            }
            
            _workingArea.SetActive(false);
            _clientDetail.SetActive(true);
        }

        public void ToClientData()
        {
            if (_clientDataRedDot.activeSelf)
            {
                RedDotManager.Instance.HideRedDot(_clientDataRedDot);
            }

            _clientData.SetActive(true);
            _memorandum.SetActive(false);
        }

        public void ToMemorandum()
        {
            if (_memorandumRedDot.activeSelf)
            {
                RedDotManager.Instance.HideRedDot(_memorandumRedDot);
            }
            
            _clientData.SetActive(false);
            _memorandum.SetActive(true);
        }
    }
}