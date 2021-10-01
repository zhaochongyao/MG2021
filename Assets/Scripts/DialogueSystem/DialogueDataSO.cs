using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue System/Dialogue")]
    public sealed class DialogueDataSO : ScriptableObject
    {
        [SerializeField] private Dialogue[] _dialogues;

        [SerializeField] private DialogueOptionSO _dialogueOptionSO;

        public Dialogue[] Dialogues => _dialogues;

        public DialogueOptionSO DialogueOptionSO => _dialogueOptionSO;
    }
}
