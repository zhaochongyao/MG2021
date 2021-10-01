using System;
using System.Collections;
using DG.Tweening;
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

        public static event Action<DialogueDataSO> ReceiveClick = delegate {};

        public void Init
        (
            Image background,
            TextMeshProUGUI text,
            float fadeOutTime,
            Ease fadeOutCurve
        )
        {
            _background = background;
            _text = text;
            _fadeOutTime = fadeOutTime;
            _fadeOutCurve = fadeOutCurve;
            _optionTarget = null;
            _lock = true;
        }

        public void OptionUpdate(DialogueDataSO optionTarget)
        {
            _optionTarget = optionTarget;
            _lock = false;
        }

        private bool _lock;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_lock == false)
            {
                StartCoroutine(ReceiveClickCo());
            }
        }
        
        private IEnumerator ReceiveClickCo()
        {
            _lock = true;
            _background
                .DOFade(0f, _fadeOutTime)
                .SetEase(_fadeOutCurve);
            _text
                .DOFade(0f, _fadeOutTime)
                .SetEase(_fadeOutCurve);

            yield return WaitCache.Seconds(_fadeOutTime);

            ReceiveClick.Invoke(_optionTarget);
        }
    }
}