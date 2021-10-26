using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using KeywordSystem;
using Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;
using Utilities.DataStructures;
using Utilities.DesignPatterns;

namespace DialogueSystem
{
    /// <summary>
    /// 对话播放器
    /// </summary>
    public sealed class DialoguePlayer : LSingleton<DialoguePlayer>
    {
        [SerializeField] private Transform _sceneCameraPanel;
        [SerializeField] private Transform _localCameraPanel;

        private List<Image> _background;
        private List<TextMeshProUGUI> _text;
        private List<DialogueOptionReceiver> _optionReceivers;

        private DialogueSystemConfigSO _config;
        private Queue<DialogueDataSO> _dialogueQueue;

        [SerializeField] private GameObject _dialogueLayoutGroupPrefab;
        private GameObject _dialogueGroup;
        private VerticalLayoutGroup _verticalLayoutGroup;

        public event Action<int, string> TextUpdate = delegate { };
        public event Action<int> TextShowBegin = delegate { };
        public event Action<int> TextShowEnd = delegate { };

        public event Action OptionBegin = delegate { };

        public event Action OptionEnd = delegate { };

        public event Action<string> DialogueEvent = delegate { };
        
        private Func<bool> _dialogueContinueCondition;
        private Func<bool> _defaultContinueCondition;

        private int _playingDialogue;

        private void Start()
        {
            InitMember();
            InitCameraSwapValues();
            StartCoroutine(InitCo());
        }

        private IEnumerator InitCo()
        {
            yield return StartCoroutine(GenerateDialogueLineCo());
            StartCoroutine(DialogueControlFlowCo());
        }

        private void InitCameraSwapValues()
        {
            // 创建相机对应两个不同屏幕的设置值
            _currentValues = new SwapCameraValues
            {
                CameraPanel = _localCameraPanel,
                LeftPadding = _config.DialogueLayoutHorizontalPadding,
                RightPadding = 0,
                TextAnchor = TextAnchor.UpperLeft,
                Pivot = new Vector2(0f, 0f)
            };

            _backupValues = new SwapCameraValues
            {
                CameraPanel = _sceneCameraPanel,
                LeftPadding = 0,
                RightPadding = _config.DialogueLayoutHorizontalPadding,
                TextAnchor = TextAnchor.UpperRight,
                Pivot = new Vector2(1f, 0f)
            };
        }

        private void InitMember()
        {
            // 初始化成员
            _dialogueQueue = new Queue<DialogueDataSO>();
            _background = new List<Image>();
            _text = new List<TextMeshProUGUI>();
            _optionReceivers = new List<DialogueOptionReceiver>();
            _eventNameSet = new HashSet<string>();

            _config = GameConfigProxy.Instance.DialogueSystemConfigSO;
            
            _optionSelected = false;
            _playingDialogue = 0;

            _defaultContinueCondition = () =>
                Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false;
        }

        private IEnumerator GenerateDialogueLineCo()
        {
            // 自适应屏幕大小，填充合适数目的对话框
            // 注意：对话框数目已在初始化时确定，若游戏中改变分辨率或屏幕比例，依然使用初始化确定的数目

            // 创建垂直布局对象
            _dialogueGroup = Instantiate
            (
                _dialogueLayoutGroupPrefab,
                _currentValues.CameraPanel
            );
            _verticalLayoutGroup = _dialogueGroup.GetComponent<VerticalLayoutGroup>();
            _verticalLayoutGroup.padding.top = _config.DialogueLayoutTopPadding;
            _verticalLayoutGroup.padding.left = _config.DialogueLayoutHorizontalPadding;
            _verticalLayoutGroup.spacing = _config.DialogueLayoutSpacing;

            Camera canvasWorldCamera = _currentValues.CameraPanel.GetComponent<Image>().canvas.worldCamera;
            float refHeight = _currentValues.CameraPanel.GetComponentInParent<CanvasScaler>().referenceResolution.y;

            int index = 0;
            while (true)
            {
                // 创建一行对话框
                GameObject imageObj = Instantiate
                (
                    _config.DialogueLinePrefab,
                    _dialogueGroup.transform
                );

                // 隐藏对话框
                Image background = imageObj.GetComponent<Image>();
                background.color = new Color(0f, 0f, 0f, 0f);
                TextMeshProUGUI text = imageObj.GetComponentInChildren<TextMeshProUGUI>();
                text.alpha = 0f;

                // 添加选项接受器
                if (imageObj.TryGetComponent(out DialogueOptionReceiver dialogueOptionReceiver) == false)
                {
                    dialogueOptionReceiver = imageObj.AddComponent<DialogueOptionReceiver>();
                }

                imageObj.AddComponent<KeywordLineIndex>().Index = index;
                // 需要等待一帧，生成对象的坐标数据才会被更新
                yield return null;

                RectTransform rectTrans = imageObj.GetComponent<RectTransform>();

                int lowerBound = (int) (canvasWorldCamera.WorldToScreenPoint(rectTrans.position).y /
                                        (GraphicOptions.Height / refHeight));

                if (lowerBound < _config.DialogueLayoutBottomPadding)
                {
                    Destroy(imageObj);
                    break;
                }

                _background.Add(background);
                _text.Add(text);
                _optionReceivers.Add(dialogueOptionReceiver);
                index++;
            }
        }

        private class SwapCameraValues
        {
            public Transform CameraPanel;
            public int LeftPadding;
            public int RightPadding;
            public Vector2 Pivot;
            public TextAnchor TextAnchor;

            public void Swap(SwapCameraValues other)
            {
                Swap(ref CameraPanel, ref other.CameraPanel);
                Swap(ref LeftPadding, ref other.LeftPadding);
                Swap(ref RightPadding, ref other.RightPadding);
                Swap(ref Pivot, ref other.Pivot);
                Swap(ref TextAnchor, ref other.TextAnchor);
            }

            private static void Swap<T>(ref T lhs, ref T rhs)
            {
                T temp = lhs;
                lhs = rhs;
                rhs = temp;
            }
        }

        private SwapCameraValues _currentValues;

        private SwapCameraValues _backupValues;

        /// <summary>
        /// 切换对话框对应的摄像机
        /// </summary>
        public void SwapCameraCanvas()
        {
            _currentValues.Swap(_backupValues);
            
            _dialogueGroup.transform.SetParent(_currentValues.CameraPanel, false);
            _verticalLayoutGroup.padding.left = _currentValues.LeftPadding;
            _verticalLayoutGroup.padding.right = _currentValues.RightPadding;
            _verticalLayoutGroup.childAlignment = _currentValues.TextAnchor;
            foreach (Image bg in _background)
            {
                bg.rectTransform.pivot = _currentValues.Pivot;

                // if (_currentValues.CameraPanel == _sceneCameraPanel && bg.rectTransform.localScale.x > 0f)
                // {
                //     bg.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
                // }
                // else if (_currentValues.CameraPanel == _localCameraPanel && bg.rectTransform.localScale.x < 0f)
                // {
                //     bg.rectTransform.localScale = new Vector3(1f, 1f, 1f);
                // }
            }
            
        }

        private IEnumerator DialogueControlFlowCo()
        {
            // 排队接受并处理一组一组的对话
            while (true)
            {
                if (_dialogueQueue.Count == 0)
                {
                    yield return null;
                }
                else
                {
                    yield return StartCoroutine(DialogueGroupDisplayCo(_dialogueQueue.Peek().Dialogues));
                    yield return StartCoroutine(CloseDialogueGroupCo());
                    if (_dialogueQueue.Peek().DialogueOptionSO != null)
                    {
                        yield return StartCoroutine(DialogueOptionDisplayCo(_dialogueQueue.Peek().DialogueOptionSO));
                    }

                    _dialogueQueue.Dequeue();
                }
            }
        }
        
        private HashSet<string> _eventNameSet;

        private bool Check(string eventName)
        {
            return _eventNameSet.Contains(eventName);
        }

        public void InvokeContinueEvent(string eventName)
        {
            _eventNameSet.Add(eventName);
        }

        private IEnumerator DialogueGroupDisplayCo(Dialogue[] dialogues)
        {
            // 处理一组对话
            for (int i = 0, curLine = 0; i < dialogues.Length; i++)
            {
                Dialogue dialogue = dialogues[i];
                StartCoroutine(DialogueLineDisplayCo(curLine, dialogue));
                yield return Wait.Seconds(_config.DialogueContinueDisplayInterval);

                _dialogueContinueCondition = 
                    string.IsNullOrEmpty(dialogue.ContinueEventName) ?
                    _defaultContinueCondition : 
                    () => Check(dialogue.ContinueEventName);

                // 等待按下按键
                while (true)
                {
                    yield return null; // 顺序不能换
                    if (_dialogueContinueCondition.Invoke())
                    {
                        break;
                    }
                }

                if (curLine == _background.Count - 1 && i != dialogues.Length - 1)
                {
                    yield return StartCoroutine(CloseDialogueGroupCo());
                    curLine = 0;
                }
                else
                {
                    curLine++;
                }
            }
        }

        private bool _optionSelected;

        private IEnumerator DialogueOptionDisplayCo(DialogueOptionSO dialogueOptionSO)
        {
            int curLine = dialogueOptionSO.NoticeDialogues.Length;
            if (curLine != 0)
            {
                yield return StartCoroutine(DialogueGroupDisplayCo(dialogueOptionSO.NoticeDialogues));
            }

            _optionSelected = false;
            OptionBegin.Invoke();
            foreach (SingleOption option in dialogueOptionSO.Options)
            {
                StartCoroutine(ActivateOption(curLine, option));
                yield return Wait.Seconds(_config.DialogueOptionDisplayInterval);

#if UNITY_EDITOR
                if (curLine == _background.Count - 1)
                {
                    Debug.LogError("选项超出上限，请不要设置过多前置对话或选项");
                }
                else
#endif
                {
                    curLine++;
                }
            }

            DialogueOptionReceiver.ReceiveClick += OnReceiveClick;
            yield return Wait.Until(() => _optionSelected);
            DialogueOptionReceiver.ReceiveClick -= OnReceiveClick;
            OptionEnd.Invoke();
            yield return StartCoroutine(CloseDialogueGroupCo());
        }

        private IEnumerator ActivateOption(int curLine, SingleOption option)
        {
            yield return StartCoroutine(DialogueLineDisplayCo(curLine, option.Description));
            _optionReceivers[curLine].OptionUpdate(option.DialogueDataSO, option.DialogueEventNameOption);
        }

        private void OnReceiveClick(DialogueDataSO dialogueDataSO, string eventName)
        {
            _optionSelected = true;
            _dialogueQueue.Enqueue(dialogueDataSO);
            
            if (string.IsNullOrEmpty(eventName) == false)
            {
                DialogueEvent.Invoke(eventName);
            }
        }

        private IEnumerator DialogueLineDisplayCo(int dialogueIndex, Dialogue dialogue)
        {
            _playingDialogue++;

            // 播放某一行的对话
            Image background = _background[dialogueIndex];
            TextMeshProUGUI text = _text[dialogueIndex];

            background.color = dialogue.CharacterDialogueStyleSO.BackGroundColor;
            text.color = dialogue.CharacterDialogueStyleSO.TextColor;
            text.alpha = 0f;
            text.text = dialogue.Text;

            // 更新事件触发
            TextUpdate.Invoke(dialogueIndex, text.text);

            background.rectTransform.sizeDelta = new Vector2
            {
                x = text.preferredWidth,
                y = background.rectTransform.sizeDelta.y
            };
            background.rectTransform.localScale = new Vector3(0f, 0f, 1f);

            // 横向缩放对话框
            background.rectTransform
                .DOScaleX(1f, _config.BackgroundScaleXTime)
                .SetEase(_config.BackgroundScaleXCurve);
            // 纵向缩放对话框
            background.rectTransform
                .DOScaleY(1f, _config.BackgroundScaleYTime)
                .SetEase(_config.BackgroundScaleYCurve);

            float scaleTime = Mathf.Max
            (
                _config.BackgroundScaleXTime,
                _config.BackgroundScaleYTime
            );
            yield return Wait.Seconds(scaleTime);

            TextShowBegin.Invoke(dialogueIndex);
            // 文字逐渐出现
            text
                .DOFade(1f, _config.TextShowTime)
                .SetEase(_config.TextShowCurve);

            yield return Wait.Seconds(_config.TextShowTime);
            TextShowEnd.Invoke(dialogueIndex);
            
            if (string.IsNullOrEmpty(dialogue.DialogueEventName) == false)
            {
                DialogueEvent.Invoke(dialogue.DialogueEventName);
            }
            
            _playingDialogue--;
        }

        private bool NoDialoguePlaying() => _playingDialogue == 0;

        private IEnumerator CloseDialogueGroupCo()
        {
            yield return Wait.Until(NoDialoguePlaying);

            // 屏幕对话框全部渐渐消失
            for (int i = 0; i < _background.Count; ++i)
            {
                _background[i]
                    .DOFade(0f, _config.DialoguePanelFadeOutTime)
                    .SetEase(_config.DialoguePanelFadeoutCurve);

                _text[i]
                    .DOFade(0f, _config.DialoguePanelFadeOutTime)
                    .SetEase(_config.DialoguePanelFadeoutCurve);
            }

            yield return Wait.Seconds(_config.DialoguePanelFadeOutTime);
        }

        /// <summary>
        /// 播放一组对话（按先到先得的顺序同步播放）
        /// </summary>
        /// <param name="dialogueDataSO"> 一组对话 </param>
        public void SendDialogue(DialogueDataSO dialogueDataSO)
        {
            _dialogueQueue.Enqueue(dialogueDataSO);
        }

        // /// <summary>
        // /// 设置对话继续播放的条件，默认为按下任意键继续
        // /// </summary>
        // /// <param name="condition"> 判定条件 </param>
        // public void SetDialogueContinueCondition(Func<bool> condition)
        // {
        //     _dialogueContinueCondition = condition;
        // }
    }
}