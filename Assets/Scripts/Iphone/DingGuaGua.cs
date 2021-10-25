using System;
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

        [SerializeField] private Color _unselectedColor;
        [SerializeField] private Color _selectedColor;

        [SerializeField] private Image _clientDataButtonBackground;
        [SerializeField] private Image _clientDataButtonIcon;
        [SerializeField] private TextMeshProUGUI _clientDataButtonText;
        
        [SerializeField] private Image _memorandumButtonBackground;
        [SerializeField] private Image _memorandumButtonIcon;
        [SerializeField] private TextMeshProUGUI _memorandumButtonText;
        
        private IphoneConfigSO _iphoneConfigSO;

        [Header("BaseData")] [SerializeField] private Image _baseDataProgressBar;
        private int _collectedData;
        private int _maxData;
        [SerializeField] private TextMeshProUGUI _clientName;

        [SerializeField] private Toggle _maleToggle;
        [SerializeField] private Toggle _femaleToggle;
        [SerializeField] private Image _maleToggleBackground;
        [SerializeField] private Image _femaleToggleBackground;
        [SerializeField] private Sprite _toggleOnImage;
        [SerializeField] private TextMeshProUGUI _maleText;
        [SerializeField] private TextMeshProUGUI _femaleText;

        [SerializeField] private Slider _ageSlider;
        // [SerializeField] private Sprite _unselectedSlider;
        // [SerializeField] private Image _sliderBackground;
        // [SerializeField] private GameObject _sliderFill;
        // [SerializeField] private GameObject _sliderToggle;
        
        [SerializeField] private TextMeshProUGUI _ageText;
        [SerializeField] private TextMeshProUGUI _ageNumText;

        [SerializeField] private TextMeshProUGUI _occupationText;
        [SerializeField] private TextMeshProUGUI _educationBackgroundText;

        [Header("ClientBackground")] [SerializeField]
        private Image _clientBackgroundProgressBar;

        private int _collectedBackground;
        private int _maxBackground;
        [SerializeField] private Transform _backgroundLayoutObject;

        [SerializeField] private GameObject _backgroundTextPrefab;
        [SerializeField] private Sprite _backgroundUnlockImage;

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

        public event Action DataCollected = delegate { }; 
        
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
                _backgroundTexts[i] = go.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            }

            KeywordListSO keywordListSO = GameConfigProxy.Instance.KeywordConfigSO.KeywordListSO;
            _maxKeyword = keywordListSO.KeywordList.Count + keywordListSO.MergeOnlyKeywordList.Count - _maxData;
            _mergeTryList = new List<string>(4);

            _keywordCollector = KeywordCollector.Instance;
            _keywordCollector.KeywordCollect += OnKeywordCollect;

            _clientName.color = _unselectedColor;
            _ageText.color = _unselectedColor;
            _ageNumText.color = _unselectedColor;
            _maleText.color = _unselectedColor;
            _femaleText.color = _unselectedColor;
            _occupationText.color = _unselectedColor;
            _educationBackgroundText.color = _unselectedColor;
            
            if (_clientData.activeSelf)
            {
                _clientDataButtonBackground.color = Color.white;
                _clientDataButtonIcon.color = Color.white;
                _clientDataButtonText.color = Color.white;

                _memorandumButtonBackground.color = Color.clear;
                _memorandumButtonIcon.color = _unselectedColor;
                _memorandumButtonText.color = _unselectedColor;
            }
            else
            {
                _clientDataButtonBackground.color =  Color.clear;
                _clientDataButtonIcon.color = _unselectedColor;
                _clientDataButtonText.color = _unselectedColor;

                _memorandumButtonBackground.color = Color.white;
                _memorandumButtonIcon.color = Color.white;
                _memorandumButtonText.color = Color.white;
            }
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
            if (_collectedBackground == _maxBackground)
            {
                DataCollected.Invoke();
            }
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
                StartCoroutine(ShowMergeResultCo(res));

                // 是否为背景
                for (int i = 0; i < _iphoneConfigSO.ClientBackgrounds.Length; ++i)
                {
                    string background = _iphoneConfigSO.ClientBackgrounds[i];
                    if (res == background)
                    {
                        _backgroundTexts[i].text = background;
                        _backgroundTexts[i].transform.parent.GetChild(0)
                            .GetComponent<Image>().sprite = _backgroundUnlockImage;
                        _backgroundTexts[i].transform.parent.GetChild(0)
                            .GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
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
                _clientName.color = _selectedColor;
                UpdateDataProgress();
            }
            else if (keyword == _iphoneConfigSO.SexText)
            {
                if (_iphoneConfigSO.IsMale)
                {
                    _maleToggle.isOn = true;
                    _maleToggleBackground.sprite = _toggleOnImage;
                    _maleText.color = Color.black;
                }
                else
                {
                    _femaleToggle.isOn = true;
                    _femaleToggleBackground.sprite = _toggleOnImage;
                    _femaleText.color = Color.black;
                }
                UpdateDataProgress();
            }
            else if (keyword == _iphoneConfigSO.Age)
            {
                _ageSlider.value = int.Parse(_iphoneConfigSO.Age);
                _ageNumText.text = _iphoneConfigSO.Age;
                _ageNumText.color = _selectedColor;
                _ageText.color = Color.black;
                UpdateDataProgress();
            }
            else if (keyword == _iphoneConfigSO.Occupation)
            {
                _occupationText.text = _iphoneConfigSO.Occupation;
                _occupationText.color = _selectedColor;
                UpdateDataProgress();
            }
            else if (keyword == _iphoneConfigSO.EducationBackground)
            {
                _educationBackgroundText.text = _iphoneConfigSO.EducationBackground;
                _educationBackgroundText.color = _selectedColor;
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

            _clientDataButtonBackground.color = Color.white;
            _clientDataButtonIcon.color = Color.white;
            _clientDataButtonText.color = Color.white;

            _memorandumButtonBackground.color = Color.clear;
            _memorandumButtonIcon.color = _unselectedColor;
            _memorandumButtonText.color = _unselectedColor;
        }

        public void ToMemorandum()
        {
            if (_memorandumRedDot.activeSelf)
            {
                RedDotManager.Instance.HideRedDot(_memorandumRedDot);
            }
            
            _clientData.SetActive(false);
            _memorandum.SetActive(true);
            
            _clientDataButtonBackground.color = Color.clear;
            _clientDataButtonIcon.color = _unselectedColor;
            _clientDataButtonText.color = _unselectedColor;

            _memorandumButtonBackground.color = Color.white;
            _memorandumButtonIcon.color = Color.white;
            _memorandumButtonText.color = Color.white;
        }
    }
}