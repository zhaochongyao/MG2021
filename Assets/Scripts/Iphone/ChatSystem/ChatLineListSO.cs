using UnityEngine;

namespace Iphone.ChatSystem
{
    [CreateAssetMenu(fileName = "ChatLineList", menuName = "Iphone/Chat Line List")]
    public class ChatLineListSO : ScriptableObject
    {
        [SerializeField] private ChatPanelSO _chatPanelSO;
        [SerializeField] private ChatLine[] _chatLineList;
        [SerializeField] private ChatOptionSO _chatOptionSO;

        [SerializeField] private string _chatEventName;
        [SerializeField] private bool _existedMessage;

        public ChatPanelSO ChatPanelSO => _chatPanelSO;
        public ChatLine[] ChatLineList => _chatLineList;
        public ChatOptionSO ChatOptionSO => _chatOptionSO;
        public string ChatEventName => _chatEventName;
        public bool ExistedMessage => _existedMessage;
    }
}