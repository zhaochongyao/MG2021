using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "DialogueOption", menuName = "ScriptableObject/Dialogue Option")]
    public class DialogueOption : ScriptableObject
    {
        [SerializeField] private SingleOption[] _options;

        public SingleOption[] Options => _options;
    }

    [System.Serializable]
    public class SingleOption
    {
        [SerializeField] private Dialogue _description;

        [SerializeField] private DialogueDataSO _dialogueData;

        public Dialogue Description => _description;

        public DialogueDataSO DialogueData => _dialogueData;
    }
}