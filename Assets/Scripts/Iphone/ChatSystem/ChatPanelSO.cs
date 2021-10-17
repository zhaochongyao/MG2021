using UnityEngine;
using UnityEngine.UI;

namespace Iphone.ChatSystem
{
    [CreateAssetMenu(fileName = "ChatPanel", menuName = "Iphone/Chat Panel")]
    public class ChatPanelSO : ScriptableObject
    {
        [SerializeField] private Sprite _thumbnail;
        [SerializeField] private string _chatName;
        [SerializeField] private bool _groupChat;

        public Sprite Thumbnail => _thumbnail;
        public string ChatName => _chatName;
        public bool GroupChat => _groupChat;
    }
}