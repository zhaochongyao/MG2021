using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.DesignPatterns;

namespace Iphone.ChatSystem
{
    public class ChatPanelContext
    {
        public TextMeshProUGUI LastTalkText { get; set; }
        public TextMeshProUGUI TimeStamp { get; set; }
        public RectTransform ChatContentViewPort { get; set; }
        public RectTransform ChatContentLayoutGroup { get; set; }
        public RectTransform ChatOptionLayoutGroup { get; set; }

        public ScrollRect ScrollRect { get; set; }
        public CanvasGroup CanvasGroup { get; set; }

        public GameObject ChatRedDot { get; set; }
    }

    public class WeChat : LSingleton<WeChat>
    {
        [SerializeField] private GameObject _friendZone;
        [SerializeField] private GameObject _chatList;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        private ChatPanelSO _curChatPanel;

        [SerializeField] private ChatPanelListSO _chatPanelListSO;

        [SerializeField] private GameObject _chatPanelOverlookPrefab;
        [SerializeField] private GameObject _chatPanelPrefab;
        [SerializeField] private GameObject _chatContentLayoutGroupPrefab;
        [SerializeField] private GameObject _chatOptionLayoutGroupPrefab;

        [SerializeField] private Transform _chatterPanelOverlookLayoutGroup;
        [SerializeField] private Transform _weChatTrans;

        [SerializeField] private Button _friendZoneButton;

        [SerializeField] private GameObject _weChatAppRedDot;

        [SerializeField] private Image _topIcon;
        [SerializeField] private Sprite _chatIcon;
        [SerializeField] private Sprite _friendZoneIcon;
        
        private Dictionary<ChatPanelSO, ChatPanelContext> _chatPanelContextMap;

        public Dictionary<ChatPanelSO, ChatPanelContext> ChatPanelContextMap => _chatPanelContextMap;

        public event Action FriendZoneEnter = delegate { };
        
        private void Start()
        {
            ChatPlayer.Instance.ChatSend += OnChatSend;
            ChatPlayer.Instance.ChatTimeUpdate += OnChatTimeUpdate;

            GameUI.UIManager.Instance.PhoneChange += OnPhoneChange;

            _chatPanelContextMap = new Dictionary<ChatPanelSO, ChatPanelContext>();
            _friendZoneButton.onClick.AddListener(ToFriendZone);

            _curChatPanel = null;
            
            // 生成聊天面板
            foreach (ChatPanelSO chatPanelSO in _chatPanelListSO.ChatPanelList)
            {
                GameObject go = Instantiate(_chatPanelOverlookPrefab, _chatterPanelOverlookLayoutGroup);

                ChatPanelContext chatPanelContext = new ChatPanelContext();

                ChatPanelOverlook chatPanelOverlook = go.GetComponent<ChatPanelOverlook>();
                chatPanelOverlook.SetThumbNail(chatPanelSO.Thumbnail);
                chatPanelOverlook.SetChatName(chatPanelSO.ChatName);
                chatPanelOverlook.SetLastTalkText("");

                RedDotManager.Instance.AddReliance(chatPanelOverlook.RedDot, _weChatAppRedDot);

                chatPanelContext.ChatRedDot = chatPanelOverlook.RedDot;
                chatPanelContext.LastTalkText = chatPanelOverlook.LastTalk;
                chatPanelContext.TimeStamp = chatPanelOverlook.TimeStamp;

                Button button = go.GetComponent<Button>();
                button.onClick.AddListener
                (
                    () =>
                    {
                        _chatList.SetActive(false);
                        _chatPanelContextMap[chatPanelSO].CanvasGroup.alpha = 1f;
                        _chatPanelContextMap[chatPanelSO].CanvasGroup.interactable = true;
                        _chatPanelContextMap[chatPanelSO].CanvasGroup.blocksRaycasts = true;

                        if (_chatPanelContextMap[chatPanelSO].ChatRedDot.activeSelf)
                        {
                            RedDotManager.Instance.HideRedDot(_chatPanelContextMap[chatPanelSO].ChatRedDot);
                        }

                        _descriptionText.text = chatPanelSO.ChatName;
                        _curChatPanel = chatPanelSO;
                        _friendZoneButton.gameObject.SetActive(false);
                    }
                );

                GameObject chatPanelObject = Instantiate(_chatPanelPrefab, _weChatTrans);

                chatPanelContext.CanvasGroup = chatPanelObject.AddComponent<CanvasGroup>();
                chatPanelContext.CanvasGroup.alpha = 0f;
                chatPanelContext.CanvasGroup.interactable = false;
                chatPanelContext.CanvasGroup.blocksRaycasts = false;

                GameObject chatContentLayoutGroupObject =
                    Instantiate(_chatContentLayoutGroupPrefab, chatPanelObject.transform);
                GameObject chatOptionLayoutGroupObject =
                    Instantiate(_chatOptionLayoutGroupPrefab, chatPanelObject.transform);

                chatPanelContext.ScrollRect =
                    chatContentLayoutGroupObject.GetComponent<ScrollRect>();
                chatPanelContext.ChatContentViewPort =
                    chatContentLayoutGroupObject.transform.GetChild(0).GetComponent<RectTransform>();
                chatPanelContext.ChatContentLayoutGroup =
                    chatContentLayoutGroupObject.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
                chatPanelContext.ChatOptionLayoutGroup =
                    chatOptionLayoutGroupObject.transform.GetChild(0).GetComponent<RectTransform>();

                _chatPanelContextMap.Add(chatPanelSO, chatPanelContext);
            }
            
            _descriptionText.text = "聊天";
            _topIcon.sprite = _chatIcon;
        }

        public void OnQuitButtonPressed()
        {
            if (_chatList.gameObject.activeSelf)
            {
                InterfaceManager.Instance.ToMainMenu();
            }
            else if (_friendZone.activeSelf)
            {
                _friendZone.SetActive(false);
                _chatList.SetActive(true);
                _friendZoneButton.gameObject.SetActive(true);
                _descriptionText.text = "聊天";
                _topIcon.sprite = _chatIcon;
            }
            else
            {
                _chatPanelContextMap[_curChatPanel].CanvasGroup.alpha = 0f;
                _chatPanelContextMap[_curChatPanel].CanvasGroup.interactable = false;
                _chatPanelContextMap[_curChatPanel].CanvasGroup.blocksRaycasts = false;
                _chatList.SetActive(true);
                _descriptionText.text = "聊天";
                _topIcon.sprite = _chatIcon;
                _curChatPanel = null;
                _friendZoneButton.gameObject.SetActive(true);
            }
        }

        private void ToFriendZone()
        {
            FriendZoneEnter.Invoke();
            
            _friendZoneButton.gameObject.SetActive(false);
            _chatList.SetActive(false);
            _friendZone.SetActive(true);
            _descriptionText.text = "朋友圈";
            _topIcon.sprite = _friendZoneIcon;
        }

        private void OnChatSend(ChatPanelSO chatPanelSO, string talk, bool existed)
        {
            if (GameUI.UIManager.Instance.PhoneOn == false && 
                _curChatPanel != null &&
                _chatPanelContextMap[_curChatPanel].ChatRedDot.activeSelf == false ||
                _curChatPanel != chatPanelSO &&
                _chatPanelContextMap[chatPanelSO].ChatRedDot.activeSelf == false)
            {
                if (existed == false)
                {
                    RedDotManager.Instance.ShowRedDot(_chatPanelContextMap[chatPanelSO].ChatRedDot);
                }
            }

            _chatPanelContextMap[chatPanelSO].LastTalkText.text = talk;
        }

        private void OnPhoneChange(bool onStatus)
        {
            if (onStatus && _curChatPanel != null && _chatPanelContextMap[_curChatPanel].ChatRedDot.activeSelf)
            {
                RedDotManager.Instance.HideRedDot(_chatPanelContextMap[_curChatPanel].ChatRedDot);
            }
        }

        private void OnChatTimeUpdate(ChatPanelSO chatPanelSO, string timeStamp)
        {
            _chatPanelContextMap[chatPanelSO].TimeStamp.text = timeStamp;
        }
    }
}