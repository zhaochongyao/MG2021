using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Iphone.ChatSystem
{
    public class ChatPanelOverlook : MonoBehaviour
    {
        [SerializeField] private Image _thumbnail;
        [SerializeField] private TextMeshProUGUI _chatName;
        [SerializeField] private TextMeshProUGUI _lastTalk;
        [SerializeField] private TextMeshProUGUI _timeStamp;

        [SerializeField] private GameObject _redDot;
        
        public TextMeshProUGUI LastTalk => _lastTalk;
        public TextMeshProUGUI TimeStamp => _timeStamp;
        public GameObject RedDot => _redDot;
        
        public void SetThumbNail(Sprite thumbnail)
        {
            _thumbnail.sprite = thumbnail;
        }

        public void SetChatName(string chatName)
        {
            _chatName.text = chatName;
        }

        public void SetLastTalkText(string lastTalk)
        {
            _lastTalk.text = lastTalk;
        }

        public void SetTimeStamp(string timeStamp)
        {
            _timeStamp.text = timeStamp;
        }
    }
}