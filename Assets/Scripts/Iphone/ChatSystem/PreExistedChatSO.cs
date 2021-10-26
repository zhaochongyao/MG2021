using UnityEngine;

namespace Iphone.ChatSystem
{
    [CreateAssetMenu(fileName = "PreExistedChat", menuName = "Iphone/Pre Existed Chat")]
    public class PreExistedChatSO : ScriptableObject
    {
        [SerializeField] private ChatLineListSO[] _preChatLineLists;

        public ChatLineListSO[] PREChatLineLists => _preChatLineLists;
    }
}