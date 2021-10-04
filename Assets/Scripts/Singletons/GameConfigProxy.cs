using DialogueSystem;
using KeywordSystem;
using ScriptableObjects;
using UnityEngine;
using Utilities.DesignPatterns;

namespace Singletons
{
    public class GameConfigProxy : GSingleton<GameConfigProxy>
    {
        [SerializeField] private DialogueSystemConfigSO _dialogueSystemConfigSO;
        [SerializeField] private KeywordConfigSO _keywordConfigSO;

        public DialogueSystemConfigSO DialogueSystemConfigSO => _dialogueSystemConfigSO;
        public KeywordConfigSO KeywordConfigSO => _keywordConfigSO;
    }
}
