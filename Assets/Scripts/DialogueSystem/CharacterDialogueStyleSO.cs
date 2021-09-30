using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "TextBackgroundColor", menuName = "ScriptableObject/TextBackgroundColor")]
    public class TextBackgroundColorSO : ScriptableObject
    {
        [SerializeField] private Color _backGroundColor;

        [SerializeField] private Color _textColor;
        
        public Color BackGroundColor => _backGroundColor;
    }
}