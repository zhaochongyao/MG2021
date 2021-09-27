using DG.Tweening;
using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "DialogueSystemConfig", menuName = "ScriptableObject/DialogueSystemConfig")]
    public class DialogueSystemConfigSO : ScriptableObject
    {
        [SerializeField] private float _backgroundScaleXTime;

        [SerializeField] private Ease _backgroundScaleXCurve;

        [SerializeField] private float _backgroundScaleYTime;

        [SerializeField] private Ease _backgroundScaleYCurve;
        
        [SerializeField] private float _textShowTime;

        [SerializeField] private Ease _textShowCurve;

        [SerializeField] private float _dialoguePanelFadeOutTime;

        [SerializeField] private Ease _dialoguePanelFadeOutCurve;

        public float BackgroundScaleXTime => _backgroundScaleXTime;

        public Ease BackgroundScaleXCurve => _backgroundScaleXCurve;

        public float BackgroundScaleYTime => _backgroundScaleYTime;

        public Ease BackgroundScaleYCurve => _backgroundScaleYCurve;

        public float TextShowTime => _textShowTime;

        public Ease TextShowCurve => _textShowCurve;

        public float DialoguePanelFadeOutTime => _dialoguePanelFadeOutTime;

        public Ease DialoguePanelFadeoutCurve => _dialoguePanelFadeOutCurve;
    }
}