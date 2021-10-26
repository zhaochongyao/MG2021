using System;
using System.Collections;
using System.IO;
using System.Text;
using DG.Tweening;
using Singletons;
using UnityEngine;
using UnityEngine.UI;
using Utilities.DataStructures;
using Utilities.DesignPatterns;

namespace Iphone
{
    public class UnlockInterface : LSingleton<UnlockInterface>
    {
        [SerializeField] private Button[] _numButtons;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private HorizontalLayoutGroup _passwordDotLayoutGroup;

        [SerializeField] private Color _wrongColor;
        [SerializeField] private float _changeTime;
        [SerializeField] private float _stayTime;

        private bool _lock;
        
        private IphoneConfigSO _iphoneConfigSO;
        private string _password;
        private StringBuilder _curInput;

        private GameObject[] _passwordDots;

        public event Action PhoneUnlock = delegate { };

        private void Start()
        {
            _iphoneConfigSO = GameConfigProxy.Instance.IphoneConfigSO;
            _password = _iphoneConfigSO.Password;

            _curInput = new StringBuilder(_password.Length);

            Transform layout = _passwordDotLayoutGroup.transform;
#if UNITY_EDITOR
            if (layout.childCount != _password.Length)
            {
                Debug.LogError("密码位数和点数不一样！");
                return;
            }
#endif
            _passwordDots = new GameObject[layout.childCount];
            for (int i = 0; i < layout.childCount; ++i)
            {
                _passwordDots[i] = layout.GetChild(i).GetChild(0).gameObject;
            }
            
            _deleteButton.onClick.AddListener(OnDelete);
            for (int i = 0; i < _numButtons.Length; ++i)
            {
                int local = i;
                _numButtons[i].onClick.AddListener(() => OnPressNum(local));
            }

            _lock = false;
        }

        private void OnPressNum(int num)
        {
            if (_lock)
            {
                return;
            }
            _passwordDots[_curInput.Length].SetActive(true);
            _curInput.Append((char) (num + '0'));
            if (_curInput.Length == _password.Length)
            {
                if (_curInput.ToString() == _password)
                {
                    Unlock();
                    PhoneUnlock.Invoke();
                }
                else
                {
                    StartCoroutine(WrongCo());
                }
            }
        }

        private void OnDelete()
        {
            _curInput.Clear();
            foreach (GameObject dot in _passwordDots)
            {
                dot.SetActive(false);
            }
        }

        private IEnumerator WrongCo()
        {
            _lock = true;
            _curInput.Clear();
            foreach (GameObject dot in _passwordDots)
            {
                if (dot.activeSelf)
                {
                    Image image = dot.GetComponent<Image>();
                    image.DOColor(_wrongColor, _changeTime);
                }
            }
            
            yield return Wait.Seconds(_changeTime + _stayTime);

            foreach (GameObject dot in _passwordDots)
            {
                if (dot.activeSelf)
                {
                    Image image = dot.GetComponent<Image>();
                    dot.SetActive(false);
                    image.color = Color.white;
                }
            }
        
            _lock = false;
        }

        private void Unlock()
        {
            InterfaceManager.Instance.ToMainMenu();
        }
    }
}