using System;
using Iphone;
using UnityEngine;
using Utilities.DesignPatterns;

namespace UI
{
    public class UIManager : LSingleton<UIManager>
    {
        [SerializeField] private CanvasGroup _iphone;

        private bool _phoneUnlocked;

        private bool _phoneOn;
        public bool PhoneOn => _phoneOn;

        public event Action<bool> PhoneChange = delegate { };
        
        private void Start()
        {
            _phoneOn = _iphone.interactable;
            _phoneUnlocked = false;
            UnlockInterface.Instance.PhoneUnlock += () => { _phoneUnlocked = true; };
        }

        public void SwitchPhone()
        {
            if (_iphone.interactable)
            {
                _iphone.interactable = false;
                _iphone.alpha = 0f;
                _iphone.blocksRaycasts = false;
                _phoneOn = false;
                PhoneChange.Invoke(false);
            }
            else
            {
                // if (_phoneUnlocked)
                // {
                //     InterfaceManager.Instance.ToMainMenu();
                // }
                _iphone.interactable = true;
                _iphone.alpha = 1f;
                _iphone.blocksRaycasts = true;
                _phoneOn = true;
                PhoneChange.Invoke(true);
            }
        }
    }
}