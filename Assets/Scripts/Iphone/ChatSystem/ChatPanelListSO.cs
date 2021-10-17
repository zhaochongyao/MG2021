using UnityEngine;

namespace Iphone.ChatSystem
{
    [CreateAssetMenu(fileName = "Chat Panel List", menuName = "Iphone/Chat Panel List")]
    public class ChatPanelListSO : ScriptableObject
    {
        [SerializeField] private ChatPanelSO[] _chatPanelList;

        public ChatPanelSO[] ChatPanelList => _chatPanelList;
    }
}