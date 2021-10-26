using System;
using System.Collections;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utilities.DataStructures;
using Utilities.DesignPatterns;

namespace GameUI
{
    public class SelfTalkManager : LSingleton<SelfTalkManager>
    {
        [SerializeField] private TextMeshProUGUI _selfTalkText;
        [SerializeField] private string _prefix;

        private StringBuilder _sb;

        [SerializeField] private float _fadeInTime;
        [SerializeField] private Ease _fadeInCurve;

        public float FadeInTime => _fadeInTime;
        public float FadeOutTime => _fadeOutTime;
        
        [SerializeField] private float _fadeOutTime;
        [SerializeField] private Ease _fadeOutCurve;

        public event Action<string> SelfTalkEnd = delegate { };

        private void Start()
        {
            _sb = new StringBuilder(_prefix);
            _selfTalkText.DOFade(0f, 0f);
        }

        public void PlaySelfTalk(string content, float lastTime)
        {
            StartCoroutine(PlaySelfTalkCo(content, lastTime));
        }

        private IEnumerator PlaySelfTalkCo(string content, float lastTime, bool over = false)
        {
            if (over)
            {
                _selfTalkText.text = content;
            }
            else
            {
                _sb.Clear();
                _sb.Append(_prefix);
                _sb.Append(content);
                _selfTalkText.text = _sb.ToString();
            }
            
            _selfTalkText
                .DOFade(1f, _fadeInTime)
                .SetEase(_fadeInCurve);

            yield return Wait.Seconds(_fadeInTime + lastTime);

            _selfTalkText
                .DOFade(0f, _fadeOutTime)
                .SetEase(_fadeOutCurve);

            yield return Wait.Seconds(_fadeInTime);
            
            SelfTalkEnd.Invoke(content);
        }
    }
}