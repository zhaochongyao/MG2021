using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "CharacterDialogueStyle", menuName = "Dialogue System/Character Dialogue Style")]
    public class CharacterDialogueStyleSO : ScriptableObject
    {
        [SerializeField] private Color _backGroundColor = new Color(1f, 1f, 1f, 1f);

        [SerializeField] private Color _textColor = new Color(0f, 0f, 0f, 1f);
        
        public Color BackGroundColor => _backGroundColor;

        public Color TextColor => _textColor;
    }
}