using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    public sealed class SingleOption
    {
        [SerializeField] private Dialogue _description;

        [SerializeField] private DialogueDataSO _dialogueDataSO;
        
        [SerializeField] private string _dialogueEventNameOption;
        public Dialogue Description => _description;

        public DialogueDataSO DialogueDataSO => _dialogueDataSO;

        public string DialogueEventNameOption => _dialogueEventNameOption;
    }
}