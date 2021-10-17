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

        [SerializeField] private GameObject _defaultInterface;
        
        private int _curInterface;

        private List<CanvasGroup> _allInterface;
        
        private void Start()
        {
            _allInterface = new List<CanvasGroup>
            {
                _mainMenu.GetComponent<CanvasGroup>(),          // 0
                _unlockInterface.GetComponent<CanvasGroup>(),   // 1
                _wechat.GetComponent<CanvasGroup>(),            // 2
                _dingGuaGua.GetComponent<CanvasGroup>()          // 3
            };
            for (int i = 0; i < _allInterface.Count; ++i)
            {
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

        public void ToWeChat() => SwitchInterface(2);

        public void ToDingGuaGua() => SwitchInterface(3);

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
