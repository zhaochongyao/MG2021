using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.DataStructures;
using Utilities.DesignPatterns;

namespace DialogueSystem
{
    /// <summary>
    /// 对话播放器
    /// </summary>
    public class DialoguePlayer : LSingleton<DialoguePlayer>
    {
        [SerializeField] private Transform _sceneCameraPanel;

        [SerializeField] private Transform _localCameraPanel;

        private List<Image> _background;

        private List<TextMeshProUGUI> _text;

        [SerializeField] private DialogueSystemConfigSO _config;

        private Queue<Dialogue[]> _dialogueQueue;

        [SerializeField] private GameObject _dialogueLayoutGroupPrefab;

        private GameObject _dialogueGroup;

        private VerticalLayoutGroup _verticalLayoutGroup;

        private Func<bool> _dialogueContinueCondition;

        private int _playingDialogue;

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
        
        private void Start()
        {
            StartCoroutine(InitCo());
        }

        private IEnumerator InitCo()
        {
            // 自适应屏幕大小，填充合适数目的对话框
            // 注意：对话框数目已在初始化时确定，若游戏中改变分辨率或屏幕比例，依然使用初始化确定的数目

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
                
            _dialogueQueue = new Queue<Dialogue[]>();
            _background = new List<Image>();
            _text = new List<TextMeshProUGUI>();

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

            while (true)
            {
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

                // 需要等待一帧，生成对象的坐标数据才会被更新
                yield return null;
                RectTransform rectTrans = imageObj.GetComponent<RectTransform>();
                int lowerBound = (int) canvasWorldCamera.WorldToScreenPoint(rectTrans.position).y;

                if (lowerBound < _config.DialogueLayoutBottomPadding)
                {
                    Destroy(imageObj);
                    break;
                }

                _background.Add(background);
                _text.Add(text);
            }
            
            // 默认为任意键继续对话
            _dialogueContinueCondition = () => Input.anyKeyDown;

            _playingDialogue = 0;
            
            StartCoroutine(DialogueControlFlowCo());
        }

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

            yield return WaitCache.Seconds
            (
                Mathf.Max
                (
                    _config.BackgroundScaleXTime,
                    _config.BackgroundScaleYTime
                )
            );

            // 文字逐渐出现
            text
                .DOFade(1f, _config.TextShowTime)
                .SetEase(_config.TextShowCurve);

            yield return WaitCache.Seconds(_config.TextShowTime);
            
            _playingDialogue--;
        }
        
        private bool NoDialoguePlaying()
        {
            return _playingDialogue == 0;
        }

        private IEnumerator CloseDialogueGroupCo()
        {
            yield return WaitCache.Until(NoDialoguePlaying);

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

            yield return WaitCache.Seconds(_config.DialoguePanelFadeOutTime);
        }
        private IEnumerator DialogueGroupDisplayCo(Dialogue[] dialogues)
        {
            // 处理一组对话
            int curLine = 0;
            foreach (Dialogue dialogue in dialogues)
            {
                 StartCoroutine(DialogueLineDisplayCo(curLine, dialogue));
                 yield return WaitCache.Seconds(_config.DialoguePlayIntervalTime);
                 
                // 等待按下按键
                while (true)
                {
                    yield return null; // 顺序不能换
                    if (_dialogueContinueCondition.Invoke())
                    {
                        break;
                    }
                }

                if (curLine == _background.Count - 1)
                {
                    yield return StartCoroutine(CloseDialogueGroupCo());
                    curLine = 0;
                }
                else
                {
                    curLine++;
                }
            }

            if (curLine != 0)
            {
                yield return StartCoroutine(CloseDialogueGroupCo());
            }
        }

        private IEnumerator DialogueControlFlowCo()
        {
            // 排队接受并处理一组一组的对话
            while (true)
            {
                yield return _dialogueQueue.Count == 0
                    ? null
                    : StartCoroutine(DialogueGroupDisplayCo(_dialogueQueue.Dequeue()));
            }
        }

        /// <summary>
        /// 播放一组对话（按先到先得的顺序同步播放）
        /// </summary>
        /// <param name="dialogueDataSO"> 一组对话 </param>
        public void SendDialogue(DialogueDataSO dialogueDataSO)
        {
            _dialogueQueue.Enqueue(dialogueDataSO.Dialogues);
        }

        /// <summary>
        /// 设置对话继续播放的条件，默认为按下任意键继续
        /// </summary>
        /// <param name="condition"> 判定条件 </param>
        public void SetDialogueContinueCondition(Func<bool> condition)
        {
            _dialogueContinueCondition = condition;
        }
    }
}