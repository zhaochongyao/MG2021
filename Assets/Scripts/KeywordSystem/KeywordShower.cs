using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
        private List<GameObject> _keywordBackgrounds;

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _clicker = gameObject.transform.parent.GetComponentInChildren<KeywordClicker>();

            _keywordCollector = KeywordCollector.Instance;
            _keywordConfigSO = GameConfigProxy.Instance.KeywordConfigSO;
            _keywordMatcher = KeywordMatcher.Instance;

            _keywordBackgroundPrefab = _keywordConfigSO.KeywordBackgroundPrefab;
            _keywordBackgrounds = new List<GameObject>();

            _highLightColorHex = ColorUtility.ToHtmlStringRGB(_keywordConfigSO.HighLightColor);
            _collectedColorHex = ColorUtility.ToHtmlStringRGB(_keywordConfigSO.CollectedColor);
        }

        // private static List<Range> MergeRange(List<Range> rangeList)
        // {
        //     List<Range> res = new List<Range> {rangeList[0]};
        //     int i, cur = 0;
        //     for (i = 1; i < rangeList.Count; ++i)
        //     {
        //         if (res[cur].Right + 1 >= rangeList[i].Left)
        //         {
        //             res[cur] = new Range(res[cur].Left, rangeList[i].Right);
        //         }
        //         else
        //         {
        //             res.Add(rangeList[i]);
        //             ++cur;
        //         }
        //     }
        //     return res;
        // }

        private void Update()
        {
            if (_text.color.grayscale < 0.5f)
            {
                RefreshDarkKeyword();
            }
            else
            {
                RefreshLightKeyword();
            }
        }

        private void RefreshDarkKeyword()
        {
            foreach (GameObject bg in _keywordBackgrounds)
            {
                Destroy(bg);
            }

            _keywordBackgrounds.Clear();

            Camera cameraOwner = GetComponentInParent<Canvas>().worldCamera;

            foreach (KeyValuePair<BoundingBox, string> pair in _clicker.ClickBoxMap)
            {
                BoundingBox box = pair.Key;
                string keyword = pair.Value;

                Vector3 position = box.Center;
                GameObject go = Instantiate(_keywordBackgroundPrefab,
                    position, Quaternion.identity, transform.parent);
                go.transform.SetAsFirstSibling();

                _keywordBackgrounds.Add(go);

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
                background.color = new Color
                {
                    r = background.color.r,
                    g = background.color.g,
                    b = background.color.b,
                    a = _text.color.a
                };
            }
        }

        private void RefreshLightKeyword()
        {
            string text = _text.text;
            text = Regex.Replace(text, @"<color=#\w{6}>", "");
            text = Regex.Replace(text, "</color>", "");
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
    }
}