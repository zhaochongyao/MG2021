using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KeywordSystem
{
    public class KeywordClicker : Text, IPointerClickHandler
    {
        private readonly Dictionary<BoundingBox, string> _clickBoxMap = new Dictionary<BoundingBox, string>();
        private readonly Dictionary<BoundingBox, string> _clickBoxMapExpanded = new Dictionary<BoundingBox, string>();

        private List<Range> _keywordLocation;

        private TextMeshProUGUI _textMeshProUGUI;
        private RectTransform _rectTrans;

        private KeywordConfigSO _keywordConfigSO;
        private KeywordCollector _keywordCollector;
        
        private string _dueText;
        
        public Dictionary<BoundingBox, string> ClickBoxMap => _clickBoxMap;

        protected override void Start()
        {
            base.Start();

            _keywordLocation = new List<Range>();
            _textMeshProUGUI = transform.parent.GetComponentInChildren<TextMeshProUGUI>();

            _keywordConfigSO = GameConfigProxy.Instance.KeywordConfigSO;
            _keywordCollector = KeywordCollector.Instance;
            
            color = new Color(0f, 0f, 0f, 0f);
            _rectTrans = GetComponent<RectTransform>();
        }

        private void Update()
        {
            raycastTarget = _textMeshProUGUI.color.a != 0f;
            string tempText = _textMeshProUGUI.text;
            tempText = Regex.Replace(tempText, @"<color=#\w{6}>", "");
            tempText = Regex.Replace(tempText, "</color>", "");
            text = tempText;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 point = eventData.pressEventCamera.ScreenToWorldPoint(eventData.position);
            foreach (KeyValuePair<BoundingBox, string> pair in _clickBoxMapExpanded)
            {
                if (pair.Key.Contains(point) && _keywordCollector.Check(pair.Value) == false)
                {
                    Debug.Log("关键词: " + pair.Value);
                    _keywordCollector.Collect(pair.Value);
                }
            }

            Debug.Log("非关键词");
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);
            
            // Debug.Log("Populate");
            if (text.Length == 0 || Application.isPlaying == false)
            {
                return;
            }

            // 清空空格和换行的字符串
            _dueText = DueString(text, out List<int> indexMap);

            // 文本更新和渲染网格不同步时会超出边界
            if (_dueText.Length * 4 != toFill.currentVertCount)
            {
                // Debug.LogWarning("aha!");
                return;
            }
            
            _clickBoxMap.Clear();
            _clickBoxMapExpanded.Clear();
            List<int> lineStart = new List<int>();

            List<UIVertex> vertexes = new List<UIVertex>(toFill.currentVertCount);
            for (int i = 0; i < toFill.currentVertCount; i++)
            {
                UIVertex tempVertex = new UIVertex();
                toFill.PopulateUIVertex(ref tempVertex, i);
                vertexes.Add(tempVertex);
            }
            
            for (int i = 0; i < cachedTextGenerator.lineCount; i++)
            {
                if (cachedTextGenerator.lines[i].startCharIdx < text.Length)
                {
                    lineStart.Add(cachedTextGenerator.lines[i].startCharIdx);
                }
            }
            lineStart.Add(int.MaxValue);

            // 所有关键词的起始和结束下标
            _keywordLocation = KeywordMatcher.Instance.Match(_dueText);

            // Debug.LogWarning("Len: " + text.Length + " " + text);
            // Debug.LogWarning("Len: " + dueText.Length + " " + dueText);
            // Debug.Log("KeywordCount: " + _keywordLocation.Count);
            //
            // Debug.LogWarning("VertexCount: " + vertexes.Count);
            
            // 生成包围盒
            int nextLine = 1;
            foreach (Range t in _keywordLocation)
            {
                int start = t.Left;
                int end = t.Right;
                int curStart = start;

                // Debug.Log("L: " + start + " R: " + end + " " + dueText.Substring(start, end - start + 1));

                while (lineStart[nextLine] <= indexMap[start])
                {
                    nextLine++;
                }

                string curKeyword = _dueText.Substring(start, end - start + 1);

                for (int i = start; i <= end; i++)
                {
                    if (i < end && indexMap[i + 1] >= lineStart[nextLine])
                    {
                        AddClickBox(vertexes, curStart, i, curKeyword);
                        curStart = i + 1;
                        nextLine++;
                    }
                }

                AddClickBox(vertexes, curStart, end, curKeyword);
            }

            foreach (KeyValuePair<BoundingBox, string> pair in _clickBoxMap)
            {
                Vector2 center = pair.Key.Center;
                Vector2 size = pair.Key.Size;
                size *= 1 + _keywordConfigSO.ClickBoxExpandRatio;
                Vector2 min = new Vector2
                {
                    x = center.x - size.x / 2,
                    y = center.y - size.y / 2
                };
                Vector2 max = new Vector2
                {
                    x = center.x + size.x / 2,
                    y = center.y + size.y / 2
                };
                BoundingBox expanded = new BoundingBox(min, max);
                _clickBoxMapExpanded.Add(expanded, pair.Value);
            }
        }

        private void AddClickBox
        (
            IReadOnlyList<UIVertex> vertexes,
            int startChar,
            int endChar,
            string keyword
        )
        {
            Vector2 min = new Vector2
            {
                x = vertexes[startChar * 4].position.x,
                y = FindPeak(vertexes, startChar * 4, endChar * 4 + 3, Mathf.Min)
            };

            Vector2 max = new Vector2
            {
                x = vertexes[endChar * 4 + 2].position.x,
                y = FindPeak(vertexes, startChar * 4, endChar * 4 + 3, Mathf.Max)
            };

            min = _rectTrans.TransformPoint(min);
            max = _rectTrans.TransformPoint(max);

            BoundingBox curBox = new BoundingBox(min, max);
            _clickBoxMap.Add(curBox, keyword);
        }

        private static string DueString(string origin, out List<int> indexMap)
        {
            StringBuilder sb = new StringBuilder();
            List<int> indexes = new List<int>();

            for (int i = 0; i < origin.Length; i++)
            {
                if (origin[i] != ' ' && origin[i] != '\n')
                {
                    sb.Append(origin[i]);
                    indexes.Add(i);
                }
            }

            indexMap = indexes;
            return sb.ToString();
        }

        private static float FindPeak
        (
            IReadOnlyList<UIVertex> vertexes,
            int startIndex,
            int endIndex,
            Func<float, float, float> extreme
        )
        {
            // 找到关键字中最高顶点或最低顶点
            float peak = vertexes[startIndex].position.y;
            for (int i = startIndex + 1; i <= endIndex; i++)
            {
                peak = extreme(peak, vertexes[i].position.y);
            }

            return peak;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            foreach (KeyValuePair<BoundingBox, string> pair in _clickBoxMap)
            {
                BoundingBox box = pair.Key;
                Gizmos.DrawWireCube(box.Center, box.Size);
            }
            Gizmos.color = Color.green;
            foreach (KeyValuePair<BoundingBox, string> pair in _clickBoxMapExpanded)
            {
                BoundingBox box = pair.Key;
                Gizmos.DrawWireCube(box.Center, box.Size);
            }
        }
    }
}