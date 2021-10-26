using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    public sealed class Dialogue
    {
        [SerializeField] private CharacterDialogueStyleSO _characterDialogueStyleSO;
        
        [TextArea, SerializeField] private string _text;

        [SerializeField] private string _dialogueEventName;

        [SerializeField] private string _continueEventName;

        public CharacterDialogueStyleSO CharacterDialogueStyleSO => _characterDialogueStyleSO;

        public string Text => _text;

        public string DialogueEventName => _dialogueEventName;

        public string ContinueEventName => _continueEventName;
    }
}