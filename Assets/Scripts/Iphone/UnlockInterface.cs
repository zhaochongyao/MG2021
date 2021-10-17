using System;
using System.Text;
using Singletons;
using UnityEngine;
using UnityEngine.UI;
using Utilities.DesignPatterns;

namespace Iphone
{
    public class UnlockInterface : LSingleton<UnlockInterface>
    {
        [SerializeField] private Button[] _numButtons;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private HorizontalLayoutGroup _passwordDotLayoutGroup;
        
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
        }

        private void OnPressNum(int num)
        {
            _passwordDots[_curInput.Length].SetActive(true);
            _curInput.Append((char) (num + '0'));
            if (_curInput.Length == _password.Length)
            {
                if (_curInput.ToString() == _password)
                {
                    Unlock();
                }
                else
                {
                    OnDelete();
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

        private void Unlock()
        {
            InterfaceManager.Instance.ToMainMenu();
            PhoneUnlock.Invoke();
        }
    }
}