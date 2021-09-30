using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "DialogueOption", menuName = "Dialogue System/Dialogue Option")]
    public class DialogueOptionSO : ScriptableObject
    {
        [SerializeField] private SingleOption[] _options;

        public SingleOption[] Options => _options;
    }

    [System.Serializable]
    public class SingleOption
    {
        [SerializeField] private Dialogue _description;

        [SerializeField] private DialogueDataSO _dialogueDataSO;

        public Dialogue Description => _description;

        public DialogueDataSO DialogueDataSO => _dialogueDataSO;
    }
}