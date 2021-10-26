using System.Collections;
using DG.Tweening;
using DialogueSystem;
using GameUI;
using Iphone;
using Iphone.ChatSystem;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
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

        [SerializeField] private GameObject _office;

        [SerializeField] private float _endSelfTalkDelay;
        [SerializeField, TextArea] private string _endSelfTalk;
        [SerializeField] private float _endSelfTalkStay;

        [SerializeField] private float _endDialogueDelay;
        [SerializeField] private DialogueDataSO _endDialogue;
            
        private bool _phoneUnlockOnce;
        private bool _friendZoneEnteredOnce;

        [SerializeField] private GameObject _rightBackground;
        [SerializeField] private GameObject _deskCamera;
        [SerializeField] private GameObject _laptopCamera;
        [SerializeField] private GameObject _drawerCamera;

        [SerializeField] private NewPlayer _newPlayer;
        
        public void ToRightBackground()
        {
            if (_rightBackground.activeSelf)
            {
                return;
            }
            _laptop.SetActive(false);
            _diary.SetActive(false);
            _desk.SetActive(false);

            _rightBackground.SetActive(true);
            _deskCamera.SetActive(false);
            _laptopCamera.SetActive(false);
            _drawerCamera.SetActive(false);
        }

        [SerializeField] private GameObject _laptop;
        [SerializeField] private GameObject _diary;
        [SerializeField] private GameObject _desk;
        
        [SerializeField] private Transform _laptopFront;
        [SerializeField] private Transform _deskFront;
        [SerializeField] private Transform _drawerFront;
        [SerializeField] private Transform _bedSide;

        public void ToDeskCamera()
        {
            if (_deskCamera.activeSelf)
            {
                return;
            }
            ToRightBackground();
            _newPlayer.MoveTowards(_deskFront.position, () =>
            {
                _rightBackground.SetActive(false);
                _deskCamera.SetActive(true);
                _laptopCamera.SetActive(false);
                _drawerCamera.SetActive(false);
                
                _desk.SetActive(true);
            });
        }

        public void ToLaptopCamera()
        {
            if (_laptopCamera.activeSelf)
            {
                return;
            }
            ToRightBackground();

            _newPlayer.MoveTowards(_laptopFront.position, () =>
            {
                _rightBackground.SetActive(false);
                _deskCamera.SetActive(false);
                _laptopCamera.SetActive(true);
                _drawerCamera.SetActive(false);
                _laptop.SetActive(true);
                ToMain();
            });
        }

        public void ToDrawerCamera()
        {
            if (_drawerCamera.activeSelf)
            {
                return;
            }
            ToRightBackground();

            _newPlayer.MoveTowards(_drawerFront.position, () =>
            {
                _rightBackground.SetActive(false);
                _deskCamera.SetActive(false);
                _laptopCamera.SetActive(false);
                _drawerCamera.SetActive(true);

                if (_drawerAnimator.enabled)
                {
                    _diary.SetActive(true);
                }
                else
                {
                    _drawerAnimator.enabled = true;
                    Wait.Delayed(() =>
                    {
                        _diary.SetActive(true);
                    }, 0.5f);
                }
            });
        }
        
        [SerializeField] private GameObject _main;
        [SerializeField] private GameObject _requirement;
        [SerializeField] private GameObject _resume;
        [SerializeField] private GameObject _score;

        public void ToMain()
        {
            _main.SetActive(true);
            _requirement.SetActive(false);
            _resume.SetActive(false);
            _score.SetActive(false);
        }

        public void ToRequirement()
        {
            _main.SetActive(false);
            _requirement.SetActive(true);
            _resume.SetActive(false);
            _score.SetActive(false);
        }

        public void ToResume()
        {
            _main.SetActive(false);
            _requirement.SetActive(false);
            _resume.SetActive(true);
            _score.SetActive(false);
        }

        public void ToScore()
        {
            _main.SetActive(false);
            _requirement.SetActive(false);
            _resume.SetActive(false);
            _score.SetActive(true);
        }

        [SerializeField] private Animator _drawerAnimator;
        
        private void OnMouseClickEvent(string eventName)
        {
        }
        
        private void Start()
        {
            MouseClicker.MouseClickEvent += OnMouseClickEvent;
            
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
            Debug.LogError("收集完毕");
            // 微信消息开始飞速变多，就像是同学开始像李华讨伐（动画）
            StartCoroutine(WakeUpInOffice());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                OnDataCollected();
            }
        }

        [SerializeField] private float _incomingChatDelay;
        [SerializeField] private GameObject _incomingChat;
        [SerializeField] private float _incomingChatStay;
        [SerializeField] private GameObject _liHuaRoom;
        [SerializeField] private Transform _spawnPoint;
        
        private IEnumerator WakeUpInOffice()
        {
            yield return Wait.Seconds(_incomingChatDelay);
            
            _incomingChat.SetActive(true);
            _incomingChat.GetComponent<Image>().color = Color.white;
            _incomingChat.GetComponent<Animator>().enabled = true;
            if (GameUI.UIManager.Instance.PhoneOn == false)
            {
                GameUI.UIManager.Instance.SwitchPhone();
            }
            InterfaceManager.Instance.ToWeChat();
            yield return Wait.Seconds(_incomingChatStay);

            _office.SetActive(true);
            _newPlayer.transform.position = _spawnPoint.position;
            _liHuaRoom.SetActive(false);
            
            yield return Wait.Seconds(_endSelfTalkDelay);
            SelfTalkManager.Instance.PlaySelfTalk(_endSelfTalk, _endSelfTalkStay);

            yield return Wait.Seconds(_endSelfTalkStay  + 
                                      SelfTalkManager.Instance.FadeInTime +
                                        SelfTalkManager.Instance.FadeOutTime);

            yield return Wait.Seconds(_endDialogueDelay);
            DialoguePlayer.Instance.SendDialogue(_endDialogue);
            
            SceneLoader.LoadScene("Chapter2");
        }
    }
}
