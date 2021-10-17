using UnityEngine;
using UnityEngine.UI;

namespace Iphone.ChatSystem
{
    [CreateAssetMenu(fileName = "Chatter", menuName = "Iphone/Chatter")]
    public class ChatterSO : ScriptableObject
    {
        [SerializeField] private string _chatterName;
        [SerializeField] private Sprite _avatar;
        public string ChatterName => _chatterName;
        public Sprite Avatar => _avatar;
    }
}