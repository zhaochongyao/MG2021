using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DialogueSystem;
using GameUI;
using Iphone;
using Iphone.ChatSystem;
using UnityEngine;
using Utilities.DataStructures;
using Utilities.DesignPatterns;

namespace StoryLine
{
    public class ChapterTwo : LSingleton<ChapterTwo>
    {
        [SerializeField] private GameObject _yuanXiaoYunTalker;
        [SerializeField] private GameObject _xiZhuRenTalker;
        
        [SerializeField] private float _maxMoveDistance;
        [SerializeField] private Ease _moveCurve;
        [SerializeField] private float _moveTime;
        
        [SerializeField] private float _dingGuaGuaNoticeDelay;
        [SerializeField] private GameObject _dingGuaGuaNotice;
        [SerializeField] private float _dingGuaGuaNoticeStay;

        [SerializeField] private float _selfTalkWithFearDelay;
        [SerializeField, TextArea] private string _selfTalkWithFear;
        [SerializeField] private float _selfTalkWithFearStay;

        [SerializeField] private float _fangZiWenChatDelay;
        [SerializeField] private ChatLineListSO _fangZiWenChat;

        [SerializeField] private ChatLineListSO _yuanXiaoYunChat;

        [SerializeField] private float _selfTalkPaperFoundDelay;
        [SerializeField, TextArea] private string[] _selfTalkPaperFound;
        [SerializeField] private float[] _selfTalkPaperFoundStay;

        [SerializeField] private float _yuanXiaoYunDialogueDelay;
        [SerializeField] private DialogueDataSO _yuanXiaoYunDialogue;

        [SerializeField] private float _selfTalkAfterYuanXiaoYunDelay;
        [SerializeField, TextArea] private string _selfTalkAfterYuanXiaoYun;
        [SerializeField] private float _selfTalkAfterYuanXiaoYunStay;

        [SerializeField] private float _xiZhuRenDialogueDelay;
        [SerializeField] private DialogueDataSO _xiZhuRenDialogue;

        [SerializeField] private float _toSecondDayDelay;
        
        [Header("平安夜")] [SerializeField] private GameObject _pingAnYePicture;
        [SerializeField] private float _happyEndDialogueDelay;
        [SerializeField] private DialogueDataSO _happyEndDialogue;

        [Header("今晚朋友圈热闹非凡")] [SerializeField] private DialogueDataSO _badEndDialogue;
        [SerializeField] private float _badEndDialogueDelay;

        [Header("成长的疼痛")] [SerializeField] private DialogueDataSO _normalEndDialogue;
        [SerializeField] private float _normalEndDialogueDelay;

        private void Start()
        {
            _eventMark = new HashSet<string>();
            
            ChatPlayer.Instance.ChatEvent += OnChatEventSend;
            DialoguePlayer.Instance.DialogueEvent += OnDialogueEventSend;

            UnlockInterface.Instance.PhoneUnlock += OnPhoneUnlock;

            SelfTalkManager.Instance.SelfTalkEnd += OnSelfTalkEnd;

            // StartCoroutine(TalkerInCo(_yuanXiaoYunTalker));
        }

        private IEnumerator TalkerInCo(GameObject talker)
        {
            // yield return talker.GetComponent<RectTransform>()
                // .DOMoveY(talker)
            yield return talker.transform
                .DOMoveY(talker.transform.position.y + _maxMoveDistance, _moveTime)
                .SetEase(_moveCurve).WaitForCompletion();
        }

        private IEnumerator TalkerOutCo(GameObject talker)
        {
            yield return talker.transform
                .DOMoveY(talker.transform.position.y - _maxMoveDistance, _moveTime)
                .SetEase(_moveCurve).WaitForCompletion();
        }

        private void OnChatEventSend(string eventName)
        {
            Debug.LogWarning(eventName);

            if (eventName == "还是假装没看到")
            {
                ChatPlayer.Instance.SendChat(_yuanXiaoYunChat);
            }
        }

        private HashSet<string> _eventMark;

        private void OnSelfTalkEnd(string content)
        {
            StartCoroutine(OnSelfTalkEndCo(content));
        }

        private IEnumerator OnSelfTalkEndCo(string content)
        { 
            if (content == _selfTalkAfterYuanXiaoYun)
            {
                if (_eventMark.Contains("C2向李华学习"))
                {
                    // 系主任
                    yield return WaitCache.Seconds(_xiZhuRenDialogueDelay);
                    yield return StartCoroutine(TalkerInCo(_xiZhuRenTalker));
                    DialoguePlayer.Instance.SendDialogue(_xiZhuRenDialogue);
                } 
                else if (_eventMark.Contains("C2输错成绩"))
                {
                    // 平安夜
                    // 系主任
                    // 第二天
                    Debug.LogError("平安夜");
                    
                    yield return StartCoroutine(ToSecondDayCo());
                    yield return WaitCache.Seconds(_happyEndDialogueDelay);
                    DialoguePlayer.Instance.SendDialogue(_happyEndDialogue);
                }
            }
        }
        
        private void OnDialogueEventSend(string eventName)
        {
            // C2寻找论文   1.1
            // C2不回复了   1.2
            
            // C2向李华学习 3.1
            // C2输错成绩   3.3
            
            // 3.1 -> 5.x
            // C2承认错误   5.1
            // C2逃避错误   5.2

            // 3.3 平安夜
            // 3.1 + 5.2 今晚朋友圈“热闹”非凡 
            // 3.1 + 5.1 成长的疼痛
            
            Debug.LogWarning(eventName);

            if (eventName == "袁小芸进门开始")
            {
                StartCoroutine(TalkerInCo(_yuanXiaoYunTalker));
                WaitCache.Delayed(() =>
                {
                    DialogueEventInvoker.Instance.InvokeContinueEvent("袁小芸进门完毕");
                }, _moveTime);
            }
            else if (eventName == "袁小芸结束")
            {
                StartCoroutine(TalkerOutCo(_yuanXiaoYunTalker));
                WaitCache.Delayed(() =>
                {
                    SelfTalkManager.Instance.PlaySelfTalk(_selfTalkAfterYuanXiaoYun, _selfTalkAfterYuanXiaoYunStay);
                }, _selfTalkAfterYuanXiaoYunDelay);
            }

            if (eventName.StartsWith("C2"))
            {
                if (eventName == "C2系主任")
                {
                    StartCoroutine(TalkerOutCo(_xiZhuRenTalker));

                    // 5
                    if (_eventMark.Contains("C2逃避错误"))
                    {
                        StartCoroutine(BadEndCo());
                    }
                    else if (_eventMark.Contains("C2承认错误"))
                    {
                        StartCoroutine(NormalEndCo());
                    }
                }
                else
                {
                    _eventMark.Add(eventName);
                }
            }
        }

        private IEnumerator BadEndCo()
        {
            // 今晚朋友圈“热闹”非凡
            Debug.LogError("今晚朋友圈“热闹”非凡");
            yield return StartCoroutine(ToSecondDayCo());
            yield return WaitCache.Seconds(_badEndDialogueDelay);
            DialoguePlayer.Instance.SendDialogue(_badEndDialogue);
        }

        private IEnumerator NormalEndCo()
        {
            // 成长的疼痛
            Debug.LogError("成长的疼痛");
            yield return StartCoroutine(ToSecondDayCo());
            yield return WaitCache.Seconds(_normalEndDialogueDelay);
            DialoguePlayer.Instance.SendDialogue(_normalEndDialogue);
        }

        private IEnumerator ToSecondDayCo()
        {
            yield return WaitCache.Seconds(_toSecondDayDelay);
        }

        private void OnPhoneUnlock()
        {
            StartCoroutine(OnPhoneUnlockCo());
        }
        
        private IEnumerator OnPhoneUnlockCo()
        {
            yield return WaitCache.Seconds(_dingGuaGuaNoticeDelay);
            _dingGuaGuaNotice.SetActive(true);
            
            yield return WaitCache.Seconds(_dingGuaGuaNoticeStay);
            while (true)
            {
                yield return null;
                if (Input.GetMouseButtonDown(0))
                {
                    break;
                }
            }
            _dingGuaGuaNotice.SetActive(false);
            
            WaitCache.Delayed(() =>
            {
                SelfTalkManager.Instance.PlaySelfTalk(_selfTalkWithFear, _selfTalkWithFearStay);
            }, _selfTalkWithFearDelay);
            
            yield return WaitCache.Seconds(_fangZiWenChatDelay);
            ChatPlayer.Instance.SendChat(_fangZiWenChat);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                OnNicePaperFound();
            }
        }

        private void OnNicePaperFound()
        {
            StartCoroutine(OnNicePaperFoundCo());
        }

        private IEnumerator OnNicePaperFoundCo()
        {
            // 内心独白
            yield return WaitCache.Seconds(_selfTalkPaperFoundDelay);
            SelfTalkManager.Instance.PlaySelfTalk(_selfTalkPaperFound[0], _selfTalkPaperFoundStay[0]);

            yield return WaitCache.Seconds(SelfTalkManager.Instance.FadeInTime + 
                                           SelfTalkManager.Instance.FadeOutTime +
                                           _selfTalkPaperFoundStay[0] +
                                           _selfTalkPaperFoundDelay);
            
            SelfTalkManager.Instance.PlaySelfTalk(_selfTalkPaperFound[1], _selfTalkPaperFoundStay[1]);

            yield return WaitCache.Seconds(SelfTalkManager.Instance.FadeInTime +
                                           SelfTalkManager.Instance.FadeOutTime +
                                           _selfTalkPaperFoundStay[1]);
            yield return WaitCache.Seconds(_yuanXiaoYunDialogueDelay);
            DialoguePlayer.Instance.SendDialogue(_yuanXiaoYunDialogue);
        }
    }
}
