using UnityEngine;

namespace Iphone.ChatSystem
{
    [CreateAssetMenu(fileName = "ChatOption", menuName = "Iphone/Chat Option")]
    public class ChatOptionSO : ScriptableObject
    {
        [SerializeField] private SingleOption[] _options;
        [SerializeField] private string _timeStampInChat;
        [SerializeField] private string _timeStampOverlook;
        [SerializeField] private string _chatEventName;

        public SingleOption[] Options => _options;
        public string TimeStampInChat => _timeStampInChat;
        public string TimeStampOverlook => _timeStampOverlook;
        public string ChatEventName => _chatEventName;
    }
}