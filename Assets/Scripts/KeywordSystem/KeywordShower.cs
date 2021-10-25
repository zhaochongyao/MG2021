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
    /// <summary>
    /// 关键词表现器
    /// </summary>
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
            _clicker = transform.GetChild(0).GetComponent<KeywordClicker>();

            // 获取对话框下标
            KeywordLineIndex keywordLineIndex = GetComponentInParent<KeywordLineIndex>();
            _lineIndex = keywordLineIndex == null ? -1 : keywordLineIndex.Index;

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

            // 显示关键词
            StartCoroutine(BuildKeyword());
        }

        private IEnumerator BuildKeyword()
        {
            // 需等待一帧，让关键词匹配器初始化完毕
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
            // 更新文本时，建立亮色字体关键词
            if (_lineIndex == lineIndex && IsLightColor())
            {
                BuildLightKeyword(newText);
            }
        }

        private void OnTextShowBegin(int lineIndex)
        {
            // 文本开始渐渐显现时，建立暗色字体关键词
            if (_lineIndex == lineIndex && IsLightColor() == false)
            {
                BuildDarkKeyword();
            }
        }

        private void OnKeywordCollect(string keyword)
        {
            // 收集关键词时，刷新关键词显示
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
            // 使用富文本标签，让关键词变色
            string text = newText;
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
                for (; i < range.Left; ++i)
                {
                    sb.Append(text[i]);
                }

                keyword.Clear();
                for (; i <= range.Right; ++i)
                {
                    keyword.Append(text[i]);
                }

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
            _text.text = sb.ToString();
        }

        private void RefreshLightKeyword(string keyword)
        {
            // 将标签中的高亮色十六进制码换成收集颜色的十六进制码
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
            // 获取关键词组件中计算的包围盒，生成关键词的背景方块
            Camera cameraOwner = GetComponentInParent<Canvas>().worldCamera;
            _keywordBackgroundMap.Clear();

            foreach (KeyValuePair<string, List<BoundingBox>> pair in _clicker.KeywordBoxMap)
            {
                string keyword = pair.Key;
                // 建立关键词和其包围盒背景方块的映射，方便更新颜色
                List<Image> backgroundList = new List<Image>(pair.Value.Count);
                _keywordBackgroundMap.Add(keyword, backgroundList);
                foreach (BoundingBox box in pair.Value)
                {
                    Vector3 position = box.Center;
                    // 生成位置使用世界坐标
                    GameObject go = ObjectPool.Spawn(_keywordBackgroundPrefab,
                        position, Quaternion.identity, transform.parent);
                    // 不遮挡字体
                    go.transform.SetAsFirstSibling();

                    RectTransform rectTrans = go.GetComponent<RectTransform>();
                    Vector3 origin = rectTrans.anchoredPosition3D;
                    rectTrans.anchoredPosition3D = new Vector3
                    {
                        x = origin.x,
                        y = origin.y,
                        z = 0f // 归零
                    };
                    
                    // 长宽需要用屏幕坐标
                    BoundingBox screenBox = new BoundingBox
                    (
                        cameraOwner.WorldToScreenPoint(box.Min),
                        cameraOwner.WorldToScreenPoint(box.Max)
                    );
                    
                    // BoundingBox screenBox = new BoundingBox
                    // (
                        // box.Min,
                        // box.Max
                    // );

                    // // 根据分辨率缩放大小
                    // CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();
                    // float refWidth = canvasScaler.referenceResolution.x;
                    // float refHeight = canvasScaler.referenceResolution.y;
                    //
                    Vector2 size = screenBox.Size;
                    // size.x = size.x * refWidth * 2 / GraphicOptions.Width;
                    // size.y = size.y * refHeight / GraphicOptions.Height;
                    rectTrans.sizeDelta = size;

                    // 设置颜色
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
            // 根据关键词，更新其对应所有方块颜色
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