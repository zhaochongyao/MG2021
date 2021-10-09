using UnityEngine;
using Utilities.DesignPatterns;

namespace UI
{
    public class UIManager : LSingleton<UIManager>
    {
        [SerializeField] private GameObject _iphone;

        public void SwitchPhone()
        {
            _iphone.SetActive(!_iphone.activeSelf);
        }
    }
}