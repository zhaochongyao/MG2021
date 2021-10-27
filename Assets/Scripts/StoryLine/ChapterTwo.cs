using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DialogueSystem;
using GameUI;
using Iphone;
using Iphone.ChatSystem;
using Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
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

        [SerializeField] private GameObject _secondDayPic;

        [Header("平安夜")] [SerializeField] private GameObject _achievementOnePic;
        [SerializeField] private float _happyEndDialogueDelay;
        [SerializeField] private DialogueDataSO _happyEndDialogue;

        [Header("今晚朋友圈热闹非凡")] [SerializeField] private GameObject _achievementTwoPic;
        [SerializeField] private DialogueDataSO _badEndDialogue;
        [SerializeField] private float _badEndDialogueDelay;

        [Header("成长的疼痛")] [SerializeField] private GameObject _achievementThreePic;
        [SerializeField] private DialogueDataSO _normalEndDialogue;
        [SerializeField] private float _normalEndDialogueDelay;

        [SerializeField] private GameObject _computer;
        [SerializeField] private GameObject _folder;
        [SerializeField] private GameObject _paper;

        [SerializeField] private AudioClip _openDoorSound;
        private AudioSource _audioSource;

        [SerializeField] private float _glitchTime;
        [SerializeField] private float _twistTime;

        [SerializeField] private GameObject _computerButton;

        [SerializeField] private float _switchToLiHuaDelay;

        [SerializeField] private float _selfTalkBeforePaperFindDelay;
        [SerializeField, TextArea] private string _selfTalkBeforePaperFind;
        [SerializeField] private float _selfTalkBeforePaperFindStay;

        public void PressComputerButton()
        {
            _computer.SetActive(_computer.activeSelf == false);
        }

        public void OpenFolder()
        {
            _folder.SetActive(true);
        }

        public void OpenPaper()
        {
            _paper.SetActive(true);
            OnNicePaperFound();
        }

        private void Start()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.loop = false;
            _audioSource.playOnAwake = false;

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
            _audioSource.PlayOneShot(_openDoorSound);
            yield return talker.transform
                .DOMoveY(talker.transform.position.y + _maxMoveDistance, _moveTime)
                .SetEase(_moveCurve).WaitForCompletion();
        }

        private IEnumerator TalkerOutCo(GameObject talker)
        {
            _audioSource.PlayOneShot(_openDoorSound);
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
            else if (eventName == "C2寻找论文")
            {
                _computerButton.SetActive(true);
                Wait.Delayed(
                    () =>
                    {
                        SelfTalkManager.Instance.PlaySelfTalk(_selfTalkBeforePaperFind, _selfTalkBeforePaperFindStay);
                    }, _selfTalkBeforePaperFindDelay);
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
                    yield return Wait.Seconds(_xiZhuRenDialogueDelay);

                    // _audioSource.PlayOneShot(_openDoorSound);

                    yield return StartCoroutine(TalkerInCo(_xiZhuRenTalker));
                    DialoguePlayer.Instance.SendDialogue(_xiZhuRenDialogue);
                }
                else if (_eventMark.Contains("C2输错成绩"))
                {
                    // 平安夜
                    // 系主任
                    // 第二天
                    Debug.LogError("平安夜");
                    _computerButton.SetActive(false);
                    _computer.SetActive(false);
                    _zhangLiLiOffice.SetActive(false);
                    _achievementOnePic.SetActive(true);
                    SelfTalkManager.Instance.PlaySelfTalk("【解锁成就：独乐乐不如众乐乐】", 2f, true);
                    PlayerPrefs.SetInt("AchievementOne", 1);
                    PlayerPrefs.Save();
                    
                    DoubleBGMController.Instance.ChangeBGM(_pingAnYeBGM);
                    yield return StartCoroutine(PlayGlitch(_leftCam));

                    yield return StartCoroutine(ToSecondDayCo());
                    yield return Wait.Seconds(_happyEndDialogueDelay);
                    DialoguePlayer.Instance.SendDialogue(_happyEndDialogue);

                    while (true)
                    {
                        if (_eventMark.Contains("C2对话播放完"))
                        {
                            break;
                        }

                        yield return null;
                    }

                    StartCoroutine(ToLiHuaHome(_end1));
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
                Wait.Delayed(() => { DialoguePlayer.Instance.InvokeContinueEvent("袁小芸进门完毕"); }, _moveTime);
            }
            else if (eventName == "袁小芸结束")
            {
                StartCoroutine(TalkerOutCo(_yuanXiaoYunTalker));
                Wait.Delayed(
                    () =>
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
            _computerButton.SetActive(false);
            _computer.SetActive(false);
            _zhangLiLiOffice.SetActive(false);
            _achievementTwoPic.SetActive(true);
            
            SelfTalkManager.Instance.PlaySelfTalk("【解锁成就：囚徒之境】", 2f, true);
            PlayerPrefs.SetInt("AchievementTwo", 1);
            PlayerPrefs.Save();
            
            DoubleBGMController.Instance.ChangeBGM(_pengYouQuanReNaoBGM);

            yield return StartCoroutine(PlayGlitch(_leftCam));

            yield return StartCoroutine(ToSecondDayCo());
            yield return Wait.Seconds(_badEndDialogueDelay);
            DialoguePlayer.Instance.SendDialogue(_badEndDialogue);

            while (true)
            {
                if (_eventMark.Contains("C2对话播放完"))
                {
                    break;
                }

                yield return null;
            }

            yield return StartCoroutine(ToLiHuaHome(_end2));
        }

        private IEnumerator NormalEndCo()
        {
            _computerButton.SetActive(false);
            _computer.SetActive(false);

            // 成长的疼痛
            Debug.LogError("成长的疼痛");

            _zhangLiLiOffice.SetActive(false);
            _liHuaHome.SetActive(true);
            _achievementThreePic.SetActive(true);

            SelfTalkManager.Instance.PlaySelfTalk("【解锁成就：成长的疼痛】", 2f, true);
            
            PlayerPrefs.SetInt("AchievementThree", 1);
            PlayerPrefs.Save();
            
            DoubleBGMController.Instance.ChangeBGM(_pengYouQuanReNaoBGM);
            yield return StartCoroutine(PlayGlitch(_leftCam));

            yield return StartCoroutine(ToSecondDayCo());
            yield return Wait.Seconds(_normalEndDialogueDelay);
            DialoguePlayer.Instance.SendDialogue(_normalEndDialogue);

            while (true)
            {
                if (_eventMark.Contains("C2对话播放完"))
                {
                    break;
                }

                yield return null;
            }

            yield return StartCoroutine(ToLiHuaHome(_end2));
        }

        [SerializeField] private GameObject _liHuaHome;
        [SerializeField] private GameObject _zhangLiLiOffice;

        [SerializeField] private Sprite _liHuaAwake;
        [SerializeField] private GameObject _liHuaPress;
        [SerializeField] private GameObject _dingBaoBook;

        [SerializeField] private float _toEndTextDelay;
        [SerializeField] private float _endStayTime;
        [SerializeField] private GameObject _end1;
        [SerializeField] private GameObject _end2;

        [SerializeField] private TextMeshProUGUI _wan;
        [SerializeField] private TextMeshProUGUI _bgmList;
        [SerializeField] private GameObject _textMsg;

        private IEnumerator ToLiHuaHome(GameObject end)
        {
            yield return Wait.Seconds(_switchToLiHuaDelay);

            _secondDayPic.SetActive(false);
            _zhangLiLiOffice.SetActive(false);
            _liHuaHome.SetActive(true);
            _liHuaPress.SetActive(true);
            
            _achievementOnePic.SetActive(false);
            _achievementTwoPic.SetActive(false);
            _achievementThreePic.SetActive(false);

            yield return StartCoroutine(PlayTwist(_leftCam));

            bool wait = true;
            
            _liHuaPress.GetComponent<Button>().onClick.AddListener(() =>
            {
                _liHuaPress.transform.GetChild(0).GetComponent<Image>().sprite = _liHuaAwake;

                _dingBaoBook.SetActive(true);
                _dingBaoBook.GetComponent<Button>().onClick.AddListener(() =>
                {
                    _dingBaoBook.transform.GetChild(0).gameObject.SetActive(false);
                    StartCoroutine(PlayGlitch(_rightCam));
                    _dingBaoBook.transform.GetChild(1).gameObject.SetActive(true);
                    wait = false;
                });
            });

            while (wait)
            {
                yield return null;
            }
            DoubleBGMController.Instance.ChangeBGM(_dingBaoShiNovelBGM);

            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    break;
                }
                yield return null;
            }

            _dingBaoBook.transform.GetChild(1).GetComponent<Image>().DOFade(0f, _fadeTime).SetEase(_fadeCurve);
            Wait.Delayed(() =>
            {
                _dingBaoBook.SetActive(false);
            }, _fadeTime);
            
            yield return Wait.Seconds(_selfTalkPaperFoundDelay);
            SelfTalkManager.Instance.PlaySelfTalk("原来……我就是李华，李华就是我", 2f);
            yield return Wait.Seconds(2f);
            SelfTalkManager.Instance.PlaySelfTalk("真是好长的一个梦……", 2f);
            yield return Wait.Seconds(2f);

            yield return Wait.Seconds(_toEndTextDelay);

            yield return StartCoroutine(Fade(end.GetComponent<Image>()));

            yield return StartCoroutine(Fade(_wan));

            yield return StartCoroutine(Fade(_bgmList));

            yield return StartCoroutine(Fade(_textMsg.GetComponent<Image>()));

            PlayerPrefs.SetInt("CurrentLevel", 0);
            PlayerPrefs.Save();
            SceneLoader.LoadScene("MainMenu");
        }

        [SerializeField] private float _fadeTime;
        [SerializeField] private Ease _fadeCurve;
        
        private IEnumerator Fade(TextMeshProUGUI text)
        {
            text.gameObject.SetActive(true);
            text.alpha = 0f;
            yield return text.DOFade(1f, _fadeTime).SetEase(_fadeCurve).WaitForCompletion();
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    break;
                }
                yield return null;
            }
            yield return text.DOFade(0f, _fadeTime).SetEase(_fadeCurve).WaitForCompletion();
        }
        
        private IEnumerator Fade(Image img)
        {
            img.gameObject.SetActive(true);
            img.color = new Color(1f, 1f, 1f, 0f);
            yield return img.DOFade(1f, _fadeTime).SetEase(_fadeCurve).WaitForCompletion();
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    break;
                }
                yield return null;
            }
            yield return img.DOFade(0f, _fadeTime).SetEase(_fadeCurve).WaitForCompletion();
        }

        [SerializeField] private GameObject _idCard;

        public void ChangeIdCard()
        {
            _idCard.SetActive(_idCard.activeSelf == false);
        }

        private IEnumerator ToSecondDayCo()
        {
            yield return Wait.Seconds(_toSecondDayDelay);
            _secondDayPic.SetActive(true);
            yield return StartCoroutine(PlayGlitch(_leftCam));
        }

        private void OnPhoneUnlock()
        {
            StartCoroutine(OnPhoneUnlockCo());
        }

        [SerializeField] private Camera _leftCam;
        [SerializeField] private Camera _rightCam;
        
        private IEnumerator PlayGlitch(Camera cam)
        {
            Glitch glitch = cam.GetComponent<Glitch>();
            glitch.enabled = true;
            glitch.Time01 = 0f;
            yield return Wait.Seconds(_glitchTime);
            glitch.enabled = false;
        }

        private IEnumerator PlayTwist(Camera cam)
        {
            Twirl twirl = cam.GetComponent<Twirl>();
            twirl.enabled = true;
            twirl.Time01 = 0f;
            yield return Wait.Seconds(_twistTime);
            twirl.enabled = false;
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

            Wait.Delayed(() => { SelfTalkManager.Instance.PlaySelfTalk(_selfTalkWithFear, _selfTalkWithFearStay); },
                _selfTalkWithFearDelay);

            yield return Wait.Seconds(_fangZiWenChatDelay);
            ChatPlayer.Instance.SendChat(_fangZiWenChat);
        }

        private void OnNicePaperFound()
        {
            StartCoroutine(OnNicePaperFoundCo());
        }

        private bool _found = false;

        private IEnumerator OnNicePaperFoundCo()
        {
            if (_found)
            {
                yield break;
            }

            _found = true;
            // 内心独白
            yield return Wait.Seconds(_selfTalkPaperFoundDelay);
            SelfTalkManager.Instance.PlaySelfTalk(_selfTalkPaperFound[0], _selfTalkPaperFoundStay[0]);

            yield return Wait.Seconds(SelfTalkManager.Instance.FadeInTime +
                                      SelfTalkManager.Instance.FadeOutTime +
                                      _selfTalkPaperFoundStay[0] +
                                      _selfTalkPaperFoundDelay);

            SelfTalkManager.Instance.PlaySelfTalk(_selfTalkPaperFound[1], _selfTalkPaperFoundStay[1]);

            yield return Wait.Seconds(SelfTalkManager.Instance.FadeInTime +
                                      SelfTalkManager.Instance.FadeOutTime +
                                      _selfTalkPaperFoundStay[1]);
            yield return Wait.Seconds(_yuanXiaoYunDialogueDelay);
            DialoguePlayer.Instance.SendDialogue(_yuanXiaoYunDialogue);
        }

        [SerializeField] private AudioClip _pingAnYeBGM;
        [SerializeField] private AudioClip _pengYouQuanReNaoBGM;
        [SerializeField] private AudioClip _dingBaoShiNovelBGM;
    }
}