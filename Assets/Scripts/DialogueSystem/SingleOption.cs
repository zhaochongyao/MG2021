using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    public sealed class SingleOption
    {
        [SerializeField] private Dialogue _description;

        [SerializeField] private DialogueDataSO _dialogueDataSO;

        public Dialogue Description => _description;

        public DialogueDataSO DialogueDataSO => _dialogueDataSO;
    }
}