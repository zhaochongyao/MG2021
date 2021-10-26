using System;
using System.Collections;
using DG.Tweening;
using Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities.DataStructures;

namespace DialogueSystem
{
    public sealed class DialogueOptionReceiver : MonoBehaviour, IPointerClickHandler
    {
        private Image _background;
        private TextMeshProUGUI _text;
        private float _fadeOutTime;
        private Ease _fadeOutCurve;

        private DialogueDataSO _optionTarget;
        private string _eventName;
        
        public static event Action<DialogueDataSO, string> ReceiveClick = delegate {};

        private void Start()
        {
            _background = GetComponent<Image>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
            
            DialogueSystemConfigSO dialogueSystemConfigSO = GameConfigProxy.Instance.DialogueSystemConfigSO;
            _fadeOutTime = dialogueSystemConfigSO.DialogueOptionFadeOutTime;
            _fadeOutCurve = dialogueSystemConfigSO.DialogueOptionFadeOutCurve;

            _optionTarget = null;
            _background.raycastTarget = false;
            
            // 所有接收器接受时，各自自动上锁
            ReceiveClick += OnReceiveClick;
        }

        private void OnReceiveClick(DialogueDataSO optionTarget, string eventName)
        {
            _background.raycastTarget = false;
        }

        public void OptionUpdate(DialogueDataSO optionTarget, string eventName)
        {
            _optionTarget = optionTarget;
            _background.raycastTarget = true;
            _eventName = eventName;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            StartCoroutine(ReceiveClickCo());
        }
        
        private IEnumerator ReceiveClickCo()
        {
            _background
                .DOFade(0f, _fadeOutTime)
                .SetEase(_fadeOutCurve);
            _text
                .DOFade(0f, _fadeOutTime)
                .SetEase(_fadeOutCurve);

            yield return WaitCache.Seconds(_fadeOutTime);

            ReceiveClick.Invoke(_optionTarget, _eventName);
        }
    }
}