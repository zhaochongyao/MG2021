using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.DataStructures;
using Utilities.DesignPatterns;
using Utilities.Random;

namespace DialogueSystem
{
    /// <summary>
    /// 对话播放器
    /// </summary>
    public class DialoguePlayer : LSingleton<DialoguePlayer>
    {
        [SerializeField] private GameObject _dialogueGroup;

        private Image[] _background;

        private TextMeshProUGUI[] _text;

        [SerializeField] private DialogueSystemConfigSO _dialogueSystemConfigSO;

        private Queue<Dialogue[]> _dialogueQueue;

        [SerializeField] private DialogueDataSO[] _dialogueDataSOs;

        private void Start()
        {
            _dialogueQueue = new Queue<Dialogue[]>();

            int count = _dialogueGroup.transform.childCount;
            _background = new Image[count];
            _text = new TextMeshProUGUI[count];

            for (int i = 0; i < count; ++i)
            {
                GameObject imageObj = _dialogueGroup.transform.GetChild(i).gameObject;
                _background[i] = imageObj.GetComponent<Image>();
                _background[i].color = new Color(0f, 0f, 0f, 0f);
                _text[i] = imageObj.GetComponentInChildren<TextMeshProUGUI>();
                _text[i].alpha = 0f;
            }

            StartCoroutine(DialogueControlFlowCo());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                DialogueDataSO dialogueData = RandomEx.Choose(_dialogueDataSOs);
                SendDialogue(dialogueData);
            }
        }

        private IEnumerator DialogueDisplayOnceCo(int dialogueIndex, Dialogue dialogue)
        {
            // 播放某一行的对话
            Image background = _background[dialogueIndex];
            TextMeshProUGUI text = _text[dialogueIndex];

            background.color = dialogue.TextBackgroundColorSO.Color;
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
                .DOScaleX(1f, _dialogueSystemConfigSO.BackgroundScaleXTime)
                .SetEase(_dialogueSystemConfigSO.BackgroundScaleXCurve);
            // 纵向缩放对话框
            background.rectTransform
                .DOScaleY(1f, _dialogueSystemConfigSO.BackgroundScaleYTime)
                .SetEase(_dialogueSystemConfigSO.BackgroundScaleYCurve);

            yield return WaitCache.Seconds(
                Mathf.Max(_dialogueSystemConfigSO.BackgroundScaleXTime,
                    _dialogueSystemConfigSO.BackgroundScaleYTime)
            );

            // 文字逐渐出现
            text.DOFade(1f, _dialogueSystemConfigSO.TextShowTime)
                .SetEase(_dialogueSystemConfigSO.TextShowCurve);

            yield return WaitCache.Seconds(_dialogueSystemConfigSO.TextShowTime);
        }

        private IEnumerator CloseDialogueCo()
        {
            // 屏幕对话框全部渐渐消失
            for (int i = 0; i < _background.Length; ++i)
            {
                _background[i]
                    .DOFade(0f, _dialogueSystemConfigSO.DialoguePanelFadeOutTime)
                    .SetEase(_dialogueSystemConfigSO.DialoguePanelFadeoutCurve);

                _text[i]
                    .DOFade(0f, _dialogueSystemConfigSO.DialoguePanelFadeOutTime)
                    .SetEase(_dialogueSystemConfigSO.DialoguePanelFadeoutCurve);
            }

            yield return WaitCache.Seconds(_dialogueSystemConfigSO.DialoguePanelFadeOutTime);
        }

        private IEnumerator DialogueDisplayAllCo(Dialogue[] dialogues)
        {
            // 处理一组对话
            int curLine = 0;
            foreach (Dialogue dialogue in dialogues)
            {
                yield return StartCoroutine(DialogueDisplayOnceCo(curLine, dialogue));

                // 等待按下按键
                while (true)
                {
                    yield return null;
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        break;
                    }
                }

                if (curLine == _background.Length - 1)
                {
                    yield return StartCoroutine(CloseDialogueCo());
                    curLine = 0;
                }
                else
                {
                    curLine++;
                }
            }

            if (curLine != 0)
            {
                yield return StartCoroutine(CloseDialogueCo());
            }
        }

        private IEnumerator DialogueControlFlowCo()
        {
            // 排队接受并处理一组一组的对话
            while (true)
            {
                yield return _dialogueQueue.Count == 0
                    ? null
                    : StartCoroutine(DialogueDisplayAllCo(_dialogueQueue.Dequeue()));
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
    }
}