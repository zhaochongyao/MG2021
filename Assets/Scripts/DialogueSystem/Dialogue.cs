using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    public sealed class Dialogue
    {
        [SerializeField] private CharacterDialogueStyleSO _characterDialogueStyleSO;
        
        [TextArea, SerializeField] private string _text;

        public CharacterDialogueStyleSO CharacterDialogueStyleSO => _characterDialogueStyleSO;

        public string Text => _text;
    }
}