using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "DialogueOption", menuName = "Dialogue System/Dialogue Option")]
    public sealed class DialogueOptionSO : ScriptableObject
    {
        [SerializeField] private Dialogue[] _noticeDialogues;
        
        [SerializeField] private SingleOption[] _options;

        public Dialogue[] NoticeDialogues => _noticeDialogues;
        
        public SingleOption[] Options => _options;
    }
}