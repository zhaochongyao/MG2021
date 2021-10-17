using UnityEngine;

namespace Iphone.ChatSystem
{
    [System.Serializable]
    public class ChatLine
    {
        [SerializeField] private ChatterSO _chatterSO;
        [SerializeField] private float _waitTime;

        [SerializeField, TextArea] private string _chatText;
        [SerializeField] private Sprite _memePic;

        [SerializeField] private string _timeStampInChat;
        [SerializeField] private string _timeStampOverlook;
        
        public ChatterSO ChatterSO => _chatterSO;
        public float WaitTime => _waitTime;

        public string ChatText => _chatText;
        public Sprite MemePic => _memePic;

        public string TimeStampInChat => _timeStampInChat;

        public string TimeStampOverlook => _timeStampOverlook;

        public ChatLine(ChatterSO chatterSO, string chatText = null, float waitTime = 0f)
        {
            _chatterSO = chatterSO;
            _chatText = chatText;
            _waitTime = waitTime;
        }

        public void SetChatText(string chatText)
        {
            _chatText = chatText;
        }
        
        public void SetMemePic(Sprite memePic)
        {
            _memePic = memePic;
        }
        
        public void SetTimeStamp(string timeStampInChat, string timeStampOverlook)
        {
            _timeStampInChat = timeStampInChat;
            _timeStampOverlook = timeStampOverlook;
        }
    }
}