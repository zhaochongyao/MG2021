using System.Collections;
using DialogueSystem;
using GameUI;
using Iphone;
using Iphone.ChatSystem;
using UnityEngine;
using Utilities.DataStructures;
using Utilities.DesignPatterns;

namespace StoryLine
{
    public class ChapterOne : LSingleton<ChapterOne>
    {
        [SerializeField] private float _initDialogueDelay;
        [SerializeField] private DialogueDataSO _initDialogue;

        [SerializeField] private float _dingGuaGuaNoticeDelay;
        [SerializeField] private GameObject _dingGuaGuaNotice;
        [SerializeField] private float _dingGuaGuaNoticeStay;

        [SerializeField] private float _zhouLingChatDelay;
        [SerializeField] private ChatLineListSO _zhouLingChat;

        [SerializeField] private float _wenWenChatDelay;
        [SerializeField] private ChatLineListSO _wenWenChat;

        [SerializeField] private float _endSelfTalkDelay;
        [SerializeField, TextArea] private string _endSelfTalk;
        [SerializeField] private float _endSelfTalkStay;

        [SerializeField] private float _endDialogueDelay;
        [SerializeField] private DialogueDataSO _endDialogue;
            
        private bool _phoneUnlockOnce;
        private bool _friendZoneEnteredOnce;
        
        private void Start()
        {
            _phoneUnlockOnce = false;
            _friendZoneEnteredOnce = true;
            
            DialoguePlayer.Instance.DialogueEvent += OnDialogueEventSend;
          
            UnlockInterface.Instance.PhoneUnlock += OnPhoneUnlock;
            InterfaceManager.Instance.WeChatOpen += OnWeChatFirstOpen;
            WeChat.Instance.FriendZoneEnter += OnFriendZoneRead;
            DingGuaGua.Instance.DataCollected += OnDataCollected;
            
            StartCoroutine(StartCo());
        }

        private IEnumerator StartCo()
        {
            yield return Wait.Seconds(_initDialogueDelay);
            DialoguePlayer.Instance.SendDialogue(_initDialogue);
        }

        private void OnDialogueEventSend(string eventName)
        {
            if (eventName == "初始对话结束")
            {
                // 播放手机提示音
            }
        }
        
        private void OnPhoneUnlock()
        {
            StartCoroutine(OnPhoneUnlockCo());
        }

        private IEnumerator OnPhoneUnlockCo()
        {
            yield return Wait.Seconds(_dingGuaGuaNoticeDelay);
            _dingGuaGuaNotice.SetActive(true);
            
            yield return Wait.Seconds(_dingGuaGuaNoticeStay);
            while (true)
            {
                yield return null;
                if (Input.GetMouseButtonDown(0))
                {
                    break;
                }
            }
            _dingGuaGuaNotice.SetActive(false);
        }

        private void OnWeChatFirstOpen()
        {
            if (_phoneUnlockOnce)
            {
                return;
            }
            _phoneUnlockOnce = true;
            _friendZoneEnteredOnce = false;

            Wait.Delayed(() =>
            {
                ChatPlayer.Instance.SendChat(_zhouLingChat);
            }, _zhouLingChatDelay);
        }

        private void OnFriendZoneRead()
        {
            if (_friendZoneEnteredOnce)
            {
                return;
            }
            _friendZoneEnteredOnce = true;
            
            Wait.Delayed(() =>
            {
                ChatPlayer.Instance.SendChat(_wenWenChat);
            }, _wenWenChatDelay);
        }

        private void OnDataCollected()
        {
            // 微信消息开始飞速变多，就像是同学开始像李华讨伐（动画）
            WakeUpInOffice();
        }

        private void WakeUpInOffice()
        {
            Wait.Delayed(() =>
            {
                SelfTalkManager.Instance.PlaySelfTalk(_endSelfTalk, _endSelfTalkStay);
            }, _endSelfTalkDelay);

            float preTime = _endSelfTalkDelay +
                            _endSelfTalkStay +
                            SelfTalkManager.Instance.FadeInTime +
                            SelfTalkManager.Instance.FadeOutTime;
            
            Wait.Delayed(() =>
            {
                DialoguePlayer.Instance.SendDialogue(_endDialogue);
            },  preTime + _endDialogueDelay);
        }

    }
}
