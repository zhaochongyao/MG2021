using UnityEngine;
using Utilities.DesignPatterns;

namespace Iphone.ChatSystem
{
    public class ChatEventListener : LSingleton<ChatEventListener>
    {
        [SerializeField] private ChatLineListSO _yuanXiaoYunChat;
        
        private void Start()
        {
            ChatPlayer.Instance.ChatEvent += OnChatEventInvoke;
        }

        private void OnChatEventInvoke(string eventName)
        {
            if (eventName == "还是假装没看到")
            {
                ChatPlayer.Instance.SendChat(_yuanXiaoYunChat);
            }
        }
    }
}