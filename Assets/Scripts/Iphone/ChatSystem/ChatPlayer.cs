﻿using System;
using System.Collections;
using System.Collections.Generic;
using KeywordSystem;
using Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.DataStructures;
using Utilities.DesignPatterns;

namespace Iphone.ChatSystem
{
    public class ChatPlayer : LSingleton<ChatPlayer>
    {
        [SerializeField] private GameObject _chatOptionPrefab;
        [SerializeField] private GameObject _otherChatLineNoNamePrefab;
        [SerializeField] private GameObject _otherChatLineWithNamePrefab;
        [SerializeField] private GameObject _selfChatLinePrefab;
        [SerializeField] private GameObject _inChatTimeStampPrefab;
        
        private IphoneConfigSO _iphoneConfigSO;
        private ChatterSO _selfChatterSO;

        public event Action<int, string> TextUpdate = delegate { };
        public event Action<ChatPanelSO, string> ChatSend = delegate { };
        public event Action<ChatPanelSO, string> ChatTimeUpdate = delegate { };

        public event Action<string> ChatEvent = delegate { }; 

        private int _chatLineID;

        private Dictionary<ChatPanelSO, ChatPanelContext> _chatPanelContextMap;

        private void Start()
        {
            _iphoneConfigSO = GameConfigProxy.Instance.IphoneConfigSO;
            _selfChatterSO = _iphoneConfigSO.SelfChatter;

            StartCoroutine(InitCo());
        }

        private IEnumerator InitCo()
        {
            yield return null;

            _chatLineID = 0x3f3f3f3f;

            _chatPanelContextMap = WeChat.Instance.ChatPanelContextMap;
        }

        /// <summary>
        /// 启动一段聊天消息
        /// </summary>
        /// <param name="chatLineListSO"> 聊天消息 </param>
        public void SendChat(ChatLineListSO chatLineListSO)
        {
            StartCoroutine(ChatDisplayCo(chatLineListSO));
        }

        private IEnumerator ChatDisplayCo(ChatLineListSO chatLineListSO)
        {
            ChatPanelSO curChatPanel = chatLineListSO.ChatPanelSO;
            RectTransform chatContentViewPort = _chatPanelContextMap[curChatPanel].ChatContentViewPort;
            RectTransform chatContentLayoutGroup = _chatPanelContextMap[curChatPanel].ChatContentLayoutGroup;
            RectTransform chatOptionLayoutGroup = _chatPanelContextMap[curChatPanel].ChatOptionLayoutGroup;
            ScrollRect scrollRect = _chatPanelContextMap[curChatPanel].ScrollRect;            
            
            float heightLimit = chatContentViewPort.sizeDelta.y;

            // 显示聊天信息
            foreach (ChatLine chatLine in chatLineListSO.ChatLineList)
            {
                yield return WaitCache.Seconds(chatLine.WaitTime);

                if (string.IsNullOrEmpty(chatLine.TimeStampInChat) == false)
                {
                    GameObject timeObject = Instantiate(_inChatTimeStampPrefab, chatContentLayoutGroup);
                    
                    TextMeshProUGUI textMeshProUGUI = timeObject.GetComponentInChildren<TextMeshProUGUI>();
                    textMeshProUGUI.text = chatLine.TimeStampInChat;

                    Image image = timeObject.GetComponentInChildren<Image>();
                    image.rectTransform.sizeDelta = new Vector2
                    {
                        x = textMeshProUGUI.preferredWidth,
                        y = image.rectTransform.sizeDelta.y
                    };
                    
                    ChatTimeUpdate.Invoke(curChatPanel, chatLine.TimeStampOverlook);
                }
                
                GameObject go;
                if (curChatPanel.GroupChat)
                {
                    go = Instantiate(_otherChatLineWithNamePrefab, chatContentLayoutGroup);

                    ChatLineWithName chatLineWithName = go.GetComponent<ChatLineWithName>();
                    chatLineWithName.Set(chatLine);
                }
                else
                {
                    go = Instantiate(_otherChatLineNoNamePrefab, chatContentLayoutGroup);
                    ChatLineNoName chatLineNoName = go.GetComponent<ChatLineNoName>();

                    chatLineNoName.Set(chatLine);
                }

                go.AddComponent<KeywordLineIndex>().Index = _chatLineID;
                yield return null;
                TextUpdate.Invoke(_chatLineID, chatLine.ChatText);
                _chatLineID++;

                yield return WaitCache.Frames(3);
            
                if (heightLimit < chatContentLayoutGroup.sizeDelta.y)
                {
                    if (chatContentLayoutGroup.pivot.y != 0f)
                    {
                        chatContentLayoutGroup.pivot = new Vector2
                        {
                            x = chatContentLayoutGroup.pivot.x,
                            y = 0f
                        };
                    }
                    chatContentLayoutGroup.localPosition = new Vector3
                    {
                        x = chatContentLayoutGroup.localPosition.x,
                        y = 0f,
                        z = chatContentLayoutGroup.localPosition.z
                    };
                    scrollRect.velocity = Vector2.zero;
                }

                if (chatLineListSO.ExistedMessage == false)
                {
                    // 播放音效
                    // 红点系统更新
                }
                string chatOverlook = curChatPanel.GroupChat ? 
                    chatLine.ChatterSO.ChatterName + ": " + chatLine.ChatText : 
                    chatLine.ChatText;
                ChatSend.Invoke(curChatPanel, chatOverlook);
            }
            
            ChatEvent.Invoke(chatLineListSO.ChatEventName);

            ChatOptionSO chatOptionSO = chatLineListSO.ChatOptionSO;
            // 显示玩家聊天选项
            if (chatOptionSO != null)
            {
                bool listening = true;
                ChatLineListSO optionTarget = null;

                ChatLine selfChatLine = new ChatLine(_selfChatterSO);
                
                void ReceiveOption(SingleOption singleOption)
                {
                    optionTarget = singleOption.TargetChatLineList;
                    selfChatLine.SetChatText(singleOption.ReplyText);
                    selfChatLine.SetMemePic(singleOption.MemePic);
                    selfChatLine.SetTimeStamp(chatOptionSO.TimeStampInChat, chatOptionSO.TimeStampOverlook);
                }

                List<GameObject> optionButtons = new List<GameObject>();

                // 产生选项按钮
                foreach (SingleOption option in chatOptionSO.Options)
                {
                    GameObject go = Instantiate(_chatOptionPrefab, chatOptionLayoutGroup);
                    optionButtons.Add(go);

                    go.GetComponentInChildren<TextMeshProUGUI>().text = option.OptionText;

                    Button button = go.GetComponent<Button>();
                    button.onClick.AddListener
                    (
                        () =>
                        {
                            listening = false;
                            ReceiveOption(option);
                        }
                    );
                }

                while (listening)
                {
                    yield return null;
                }

                // 点击后，销毁所有选项按钮
                foreach (GameObject go in optionButtons)
                {
                    Destroy(go);
                }

                optionButtons.Clear();

                if (string.IsNullOrEmpty(selfChatLine.ChatText) == false)
                {
                    if (string.IsNullOrEmpty(selfChatLine.TimeStampInChat) == false)
                    {
                        GameObject timeObject = Instantiate(_inChatTimeStampPrefab, chatContentLayoutGroup);
                    
                        TextMeshProUGUI textMeshProUGUI = timeObject.GetComponentInChildren<TextMeshProUGUI>();
                        textMeshProUGUI.text = selfChatLine.TimeStampInChat;

                        Image image = timeObject.GetComponentInChildren<Image>();
                        image.rectTransform.sizeDelta = new Vector2
                        {
                            x = textMeshProUGUI.preferredWidth,
                            y = image.rectTransform.sizeDelta.y
                        };
                    
                        ChatTimeUpdate.Invoke(curChatPanel, selfChatLine.TimeStampOverlook);
                    }
                    
                    GameObject selfChat = Instantiate(_selfChatLinePrefab, chatContentLayoutGroup);

                    ChatLineNoName chatLineNoName = selfChat.GetComponent<ChatLineNoName>();
                    chatLineNoName.Set(selfChatLine);

                    yield return WaitCache.Frames(3);

                    if (heightLimit < chatContentLayoutGroup.sizeDelta.y)
                    {
                        if (chatContentLayoutGroup.pivot.y != 0f)
                        {
                            chatContentLayoutGroup.pivot = new Vector2
                            {
                                x = chatContentLayoutGroup.pivot.x,
                                y = 0f
                            };
                        }
                        chatContentLayoutGroup.localPosition = new Vector3
                        {
                            x = chatContentLayoutGroup.localPosition.x,
                            y = 0f,
                            z = chatContentLayoutGroup.localPosition.z
                        };
                        scrollRect.velocity = Vector2.zero;
                    }

                    ChatSend.Invoke(curChatPanel, selfChatLine.ChatText);
                }
                
                ChatEvent.Invoke(chatOptionSO.ChatEventName);

                // 递归开启选项对应的下一段消息（如有）
                if (optionTarget != null)
                {
                    yield return StartCoroutine(ChatDisplayCo(optionTarget));
                }
            }
        }
    }
}