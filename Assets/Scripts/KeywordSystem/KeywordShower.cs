using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DialogueSystem;
using Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace KeywordSystem
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class KeywordShower : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        private KeywordClicker _clicker;

        private KeywordCollector _keywordCollector;
        private KeywordConfigSO _keywordConfigSO;
        private KeywordMatcher _keywordMatcher;

        private string _highLightColorHex;
        private string _collectedColorHex;

        private GameObject _keywordBackgroundPrefab;

        private Dictionary<string, List<Image>> _keywordBackgroundMap;

        private int _lineIndex;

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _clicker = gameObject.transform.parent.GetComponentInChildren<KeywordClicker>();

            DialogueLineIndex dialogueLineIndex = GetComponentInParent<DialogueLineIndex>();
            _lineIndex = dialogueLineIndex == null ? -1 : dialogueLineIndex.Index;

            _keywordCollector = KeywordCollector.Instance;
            _keywordConfigSO = GameConfigProxy.Instance.KeywordConfigSO;
            _keywordMatcher = KeywordMatcher.Instance;

            _keywordBackgroundPrefab = _keywordConfigSO.KeywordBackgroundPrefab;
            _keywordBackgroundMap = new Dictionary<string, List<Image>>();

            DialoguePlayer.Instance.TextUpdate += OnTextUpdate;
            DialoguePlayer.Instance.TextShowBegin += OnTextShowBegin;
            _keywordCollector.KeywordCollect += OnKeywordCollect;

            _highLightColorHex = ColorUtility.ToHtmlStringRGB(_keywordConfigSO.HighLightColor);
            _collectedColorHex = ColorUtility.ToHtmlStringRGB(_keywordConfigSO.CollectedColor);

            StartCoroutine(BuildKeyword());
        }

        private IEnumerator BuildKeyword()
        {
            yield return null;
            if (IsLightColor())
            {
                BuildLightKeyword(_text.text);
            }
            else
            {
                BuildDarkKeyword();
            }
        }

        private bool IsLightColor()
        {
            return _text.color.grayscale > 0.5f;
        }

        private void OnTextUpdate(int lineIndex, string newText)
        {
            if (_lineIndex == lineIndex && IsLightColor())
            {
                BuildLightKeyword(newText);
            }
        }

        private void OnTextShowBegin(int lineIndex)
        {
            if (_lineIndex == lineIndex && IsLightColor() == false)
            {
                BuildDarkKeyword();
            }
        }

        private void OnKeywordCollect(string keyword)
        {
            if (IsLightColor())
            {
                RefreshLightKeyword(keyword);
            }
            else
            {
                RefreshDarkKeyword(keyword);
            }
        }

        private void BuildLightKeyword(string newText)
        {
            string text = newText;
            // Debug.Log(text + " " + text.Length);

            List<Range> ori = _keywordMatcher.Match(text);
            if (ori.Count == 0)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            StringBuilder keyword = new StringBuilder();
            int i = 0;

            foreach (Range range in ori)
            {
                // Debug.LogWarning(range.Left + " " + range.Right);
                for (; i < range.Left; ++i)
                {
                    sb.Append(text[i]);
                }

                keyword.Clear();
                for (; i <= range.Right; ++i)
                {
                    keyword.Append(text[i]);
                }
                // Debug.LogWarning(keyword);

                string keywordColorHex = _keywordCollector.Check(keyword.ToString())
                    ? _collectedColorHex
                    : _highLightColorHex;
                sb.Append("<color=#" + keywordColorHex + ">");
                sb.Append(keyword);
                sb.Append("</color>");
            }

            for (; i < text.Length; ++i)
            {
                sb.Append(text[i]);
            }

            // Debug.LogError(_text.text + "\n" + sb);
            _text.text = sb.ToString();
        }

        private void RefreshLightKeyword(string keyword)
        {
            string text = _text.text;
            StringBuilder sb = new StringBuilder();
            sb.Append("<color=#");
            sb.Append(_highLightColorHex);
            sb.Append(">");
            sb.Append(keyword);
            sb.Append("</color>");

            string pattern = sb.ToString();
            for (int i = 8; i < 14; ++i)
            {
                sb[i] = _collectedColorHex[i - 8];
            }

            string replace = sb.ToString();
            _text.text = Regex.Replace(text, pattern, replace);
        }

        private void BuildDarkKeyword()
        {
            Camera cameraOwner = GetComponentInParent<Canvas>().worldCamera;
            _keywordBackgroundMap.Clear();

            foreach (KeyValuePair<string, List<BoundingBox>> pair in _clicker.KeywordBoxMap)
            {
                string keyword = pair.Key;
                List<Image> backgroundList = new List<Image>(pair.Value.Count);
                _keywordBackgroundMap.Add(keyword, backgroundList);
                foreach (BoundingBox box in pair.Value)
                {
                    Vector3 position = box.Center;
                    GameObject go = ObjectPool.Spawn(_keywordBackgroundPrefab,
                        position, Quaternion.identity, transform.parent);
                    go.transform.SetAsFirstSibling();

                    RectTransform rectTrans = go.GetComponent<RectTransform>();
                    Vector3 origin = rectTrans.anchoredPosition3D;
                    rectTrans.anchoredPosition3D = new Vector3
                    {
                        x = origin.x,
                        y = origin.y,
                        z = 0f // 归零
                    };

                    BoundingBox screenBox = new BoundingBox
                    (
                        cameraOwner.WorldToScreenPoint(box.Min),
                        cameraOwner.WorldToScreenPoint(box.Max)
                    );

                    // 根据分辨率缩放大小
                    CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();
                    float refWidth = canvasScaler.referenceResolution.x;
                    float refHeight = canvasScaler.referenceResolution.y;

                    // Debug.Log(refWidth + " " + refHeight);
                    // Debug.Log(GraphicOptions.Width + " " + GraphicOptions.Height);
                    Vector2 size = screenBox.Size;
                    size.x = size.x * refWidth * 2 / GraphicOptions.Width;
                    size.y = size.y * refHeight / GraphicOptions.Height;
                    rectTrans.sizeDelta = size;

                    Image background = go.GetComponent<Image>();
                    background.color = _keywordCollector.Check(keyword)
                        ? _keywordConfigSO.CollectedColor
                        : _keywordConfigSO.HighLightColor;

                    backgroundList.Add(background);
                }
            }
        }

        private void RefreshDarkKeyword(string keyword)
        {
            if (_keywordBackgroundMap.TryGetValue(keyword, out List<Image> backgroundList))
            {
                foreach (Image background in backgroundList)
                {
                    background.color = new Color
                    {
                        r = _keywordConfigSO.CollectedColor.r,
                        g = _keywordConfigSO.CollectedColor.g,
                        b = _keywordConfigSO.CollectedColor.b,
                        a = background.color.a
                    };
                }
            }
        }
    }
}