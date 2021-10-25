using UnityEngine;

namespace Iphone.ChatSystem
{
    [System.Serializable]
    public class SingleOption
    {
        [SerializeField, TextArea] private string _optionText;
        [SerializeField] private ChatLineListSO _targetChatLineList;
        [SerializeField] private string _replyText;
        [SerializeField] private Sprite _memePic;
        [SerializeField] private string _chatEventName;


        public string OptionText => _optionText;
        public ChatLineListSO TargetChatLineList => _targetChatLineList;
        public string ReplyText => _replyText;
        public Sprite MemePic => _memePic;
        public string ChatEventName => _chatEventName;
    }
}