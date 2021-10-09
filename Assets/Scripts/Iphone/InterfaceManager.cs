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
        
        private int _curInterface;

        private List<GameObject> _allInterface;
        
        private void Start()
        {
            _curInterface = 1;
            _allInterface = new List<GameObject>
            {
                _mainMenu,          // 0
                _unlockInterface,   // 1
                _wechat,            // 2
                _dingGuaGua          // 3
            };
        }

        public void ToMainMenu() => SwitchInterface(0);

        public void ToUnlockInterface() => SwitchInterface(1);

        public void ToWeChat() => SwitchInterface(2);

        public void ToDingGuaGua() => SwitchInterface(3);

        private void SwitchInterface(int index)
        {
#if UNITY_EDITOR
            if (index == _curInterface)
            {
                Debug.LogError("重复进入同一界面");
                return;
            }
#endif
            _allInterface[_curInterface].SetActive(false);
            _allInterface[index].SetActive(true);
            _curInterface = index;
        }
    }
}
