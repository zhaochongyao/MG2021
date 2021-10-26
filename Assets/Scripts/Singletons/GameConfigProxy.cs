using DialogueSystem;
using Iphone;
using KeywordSystem;
using UnityEngine;
using Utilities.DesignPatterns;

namespace Singletons
{
    public class GameConfigProxy : LSingleton<GameConfigProxy>
    {
        [SerializeField] private DialogueSystemConfigSO _dialogueSystemConfigSO;
        [SerializeField] private KeywordConfigSO _keywordConfigSO;
        [SerializeField] private IphoneConfigSO _iphoneConfigSO;
        
        public DialogueSystemConfigSO DialogueSystemConfigSO => _dialogueSystemConfigSO;
        public KeywordConfigSO KeywordConfigSO => _keywordConfigSO;
        public IphoneConfigSO IphoneConfigSO => _iphoneConfigSO;
    }
}
