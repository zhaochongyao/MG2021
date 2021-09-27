using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    public class Dialogue
    {
        [SerializeField] private TextBackgroundColorSO _textBackgroundColorSO;
        
        [TextArea, SerializeField] private string _text;

        public TextBackgroundColorSO TextBackgroundColorSO => _textBackgroundColorSO;

        public string Text => _text;
    }
    
    [CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObject/Dialogue")]
    public class DialogueDataSO : ScriptableObject
    {
        [SerializeField] private Dialogue[] _dialogues;

        public Dialogue[] Dialogues => _dialogues;
    }
}
