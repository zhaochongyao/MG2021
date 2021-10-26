using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities.DesignPatterns;

namespace Iphone
{
    public class InterfaceManager : LSingleton<InterfaceManager>
    {
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _unlockInterface;
        [SerializeField] private GameObject _wechat;
        [SerializeField] private GameObject _dingGuaGua;
        [SerializeField] private GameObject _album;
        [SerializeField] private GameObject _calendar;
        
        [SerializeField] private GameObject _defaultInterface;
        
        private int _curInterface;

        private List<CanvasGroup> _allInterface;

        public event Action WeChatOpen = delegate { };
        
        private void Start()
        {
            _allInterface = new List<CanvasGroup>
            {
                _mainMenu.GetComponent<CanvasGroup>(),          // 0
                _unlockInterface.GetComponent<CanvasGroup>(),   // 1
                _wechat.GetComponent<CanvasGroup>(),            // 2
                _dingGuaGua.GetComponent<CanvasGroup>(),        // 3
                _album.GetComponent<CanvasGroup>(),             // 4
                _calendar.GetComponent<CanvasGroup>()                // 5
            };
            for (int i = 0; i < _allInterface.Count; ++i)
            {
                _allInterface[i].gameObject.SetActive(true);
                if (_allInterface[i].gameObject == _defaultInterface)
                {
                    Show(_allInterface[i]);
                    _curInterface = i;
                }
                else
                {
                    Hide(_allInterface[i]);
                }
            }
        }

        public void ToMainMenu() => SwitchInterface(0);

        public void ToUnlockInterface() => SwitchInterface(1);

        public void ToWeChat()
        {
            WeChatOpen.Invoke();
            SwitchInterface(2);
        }

        public void ToDingGuaGua() => SwitchInterface(3);

        public void ToAlbum() => SwitchInterface(4);

        public void ToCalendar() => SwitchInterface(5);

        private void SwitchInterface(int index)
        {
            if (index == _curInterface)
            {
                return;
            }
            Hide(_allInterface[_curInterface]);
            Show(_allInterface[index]);
            _curInterface = index;
        }

        private void Show(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        private void Hide(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
