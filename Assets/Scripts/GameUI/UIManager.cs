using System;
using UnityEngine;
using UnityEngine.UI;
using Utilities.DesignPatterns;

namespace GameUI
{
    public class UIManager : LSingleton<UIManager>
    {
        [SerializeField] private CanvasGroup _iphone;

        [SerializeField] private Image _phoneButtonIcon;

        [SerializeField] private Sprite _phoneOnIcon;
        [SerializeField] private Sprite _phoneShutIcon;
        
        private bool _phoneOn;
        public bool PhoneOn => _phoneOn;

        public event Action<bool> PhoneChange = delegate { };
        
        private void Start()
        {
            _phoneOn = _iphone.interactable;
            _phoneButtonIcon.sprite = _phoneOn ? _phoneShutIcon : _phoneOnIcon;
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
                _phoneButtonIcon.sprite = _phoneOnIcon;
            }
            else
            {
                _iphone.interactable = true;
                _iphone.alpha = 1f;
                _iphone.blocksRaycasts = true;
                _phoneOn = true;
                PhoneChange.Invoke(true);
                _phoneButtonIcon.sprite = _phoneShutIcon;
            }
        }
    }
}