using DG.Tweening;
using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "DialogueSystemConfig", menuName = "Dialogue System/Dialogue System Config")]
    public sealed class DialogueSystemConfigSO : ScriptableObject
    {
        [SerializeField] private float _dialogueContinueDisplayInterval;

        [SerializeField] private float _dialogueOptionDisplayInterval;

        [SerializeField] private float _dialogueOptionFadeOutTime;

        [SerializeField] private Ease _dialogueOptionFadeOutCurve;
        
        [SerializeField] private float _backgroundScaleXTime;

        [SerializeField] private Ease _backgroundScaleXCurve;

        [SerializeField] private float _backgroundScaleYTime;

        [SerializeField] private Ease _backgroundScaleYCurve;
        
        [SerializeField] private float _textShowTime;

        [SerializeField] private Ease _textShowCurve;

        [SerializeField] private float _dialoguePanelFadeOutTime;

        [SerializeField] private Ease _dialoguePanelFadeOutCurve;

        [SerializeField] private GameObject _dialogueLinePrefab;

        [SerializeField] private int _dialogueLayoutTopPadding;

        [SerializeField] private int _dialogueLayoutHorizontalPadding;

        [SerializeField] private int _dialogueLayoutBottomPadding;

        [SerializeField] private int _dialogueLayoutSpacing;

        public float DialogueContinueDisplayInterval => _dialogueContinueDisplayInterval;

        public float DialogueOptionDisplayInterval => _dialogueOptionDisplayInterval;

        public float DialogueOptionFadeOutTime => _dialogueOptionFadeOutTime;

        public Ease DialogueOptionFadeOutCurve => _dialogueOptionFadeOutCurve;
        
        public float BackgroundScaleXTime => _backgroundScaleXTime;

        public Ease BackgroundScaleXCurve => _backgroundScaleXCurve;

        public float BackgroundScaleYTime => _backgroundScaleYTime;

        public Ease BackgroundScaleYCurve => _backgroundScaleYCurve;

        public float TextShowTime => _textShowTime;

        public Ease TextShowCurve => _textShowCurve;

        public float DialoguePanelFadeOutTime => _dialoguePanelFadeOutTime;

        public Ease DialoguePanelFadeoutCurve => _dialoguePanelFadeOutCurve;
        
        public GameObject DialogueLinePrefab => _dialogueLinePrefab;
        
        public int DialogueLayoutTopPadding => _dialogueLayoutTopPadding;

        public int DialogueLayoutBottomPadding => _dialogueLayoutBottomPadding;
        
        public int DialogueLayoutHorizontalPadding => _dialogueLayoutHorizontalPadding;
        
        public int DialogueLayoutSpacing => _dialogueLayoutSpacing;
    }
}