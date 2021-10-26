using System;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
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

        [SerializeField] private GameObject _pauseMask;
        [SerializeField] private GameObject _pauseMenu;
        private bool _pause = false;
        
        public void PressPause()
        {
            _pause = !_pause;
            if (_pause)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }

        public void Resume()
        {
            Time.timeScale = 1f;
            _pauseMask.SetActive(false);
            _pauseMenu.SetActive(false);
        }

        public void Pause()
        {
            Time.timeScale = 0f;
            _pauseMask.SetActive(true);
            _pauseMenu.SetActive(true);
        }

        public void ToMainMenu()
        {
            Time.timeScale = 1f;
            SceneLoader.LoadScene("MainMenu");
        }
        
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