using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "TextBackgroundColor", menuName = "ScriptableObject/TextBackgroundColor")]
    public class TextBackgroundColorSO : ScriptableObject
    {
        [SerializeField] private Color _color;

        public Color Color => _color;
    }
}