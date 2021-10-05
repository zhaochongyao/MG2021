using System;
using System.Collections.Generic;
using System.Text;
using DialogueSystem;
using Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KeywordSystem
{
    public class KeywordClicker : Text, IPointerClickHandler
    {
        private readonly Dictionary<string, List<BoundingBox>> _keywordBoxMap = new Dictionary<string, List<BoundingBox>>();
        
        private TextMeshProUGUI _textMeshProUGUI;
        private RectTransform _rectTrans;

        private KeywordConfigSO _keywordConfigSO;
        private KeywordCollector _keywordCollector;
        private KeywordMatcher _keywordMatcher;

        private string _cleanText;

        private int _lineIndex;

        public Dictionary<string, List<BoundingBox>> KeywordBoxMap => _keywordBoxMap;
        
        protected override void Start()
        {
            base.Start();

            Transform parent = transform.parent;
            _textMeshProUGUI = parent.GetComponentInChildren<TextMeshProUGUI>();
            
            bool receiveOptionEvent = parent.GetComponent<DialogueOptionReceiver>() != null;
            if (receiveOptionEvent)
            {
                DialoguePlayer.Instance.OptionBegin += OnOptionBegin;
                DialoguePlayer.Instance.OptionEnd += OnOptionEnd;
            }
            _raycastOff = false;
            
            DialogueLineIndex dialogueLineIndex = GetComponentInParent<DialogueLineIndex>();
            _lineIndex = dialogueLineIndex == null ? -1 : dialogueLineIndex.Index; 
            
            _keywordConfigSO = GameConfigProxy.Instance.KeywordConfigSO;
            _keywordCollector = KeywordCollector.Instance;
            _keywordMatcher = KeywordMatcher.Instance;

            DialoguePlayer.Instance.TextUpdate += OnTextUpdate;
            DialoguePlayer.Instance.TextShowBegin += OnTextShowBegin;
            DialoguePlayer.Instance.TextShowEnd += OnTextShowEnd;
            
            color = new Color(0f, 0f, 0f, 0f);
            _rectTrans = GetComponent<RectTransform>();

            _textShowEnd = true;
        }

        private bool _raycastOff;

        private void OnOptionBegin()
        {
            _raycastOff = true;
        }

        private void OnOptionEnd()
        {
            _raycastOff = false;
        }

        private void Update()
        {
            if (_raycastOff)
            {
                if (raycastTarget)
                {
                    raycastTarget = false;
                }
                return;
            }
            
            if (_textMeshProUGUI.color.a != 0f && raycastTarget == false)
            {
                raycastTarget = true;
            }
            else if (_textMeshProUGUI.color.a == 0f && raycastTarget)
            {
                raycastTarget = false;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 point = eventData.pressEventCamera.ScreenToWorldPoint(eventData.position);
            foreach (KeyValuePair<string, List<BoundingBox>> pair in _keywordBoxMap)
            {
                string keyword = pair.Key;
                if (_keywordCollector.Check(keyword))
                {
                    continue;
                }
                foreach (BoundingBox boundingBox in pair.Value)
                {
                    BoundingBox box = new BoundingBox
                    (
                        boundingBox.Center,
                        boundingBox.Size,
                        1 + _keywordConfigSO.ClickBoxExpandRatio
                    );
                    
                    if (box.Contains(point))
                    {
                        Debug.Log("关键词: " + keyword);
                        _keywordCollector.Collect(keyword);
                    }
                }
            }
            Debug.Log("非关键词");
        }
        
        private void OnTextUpdate(int lineIndex, string newText)
        {
            if (_lineIndex == lineIndex)
            {
                text = newText;
            }
        }

        private bool _textShowEnd;

        private void OnTextShowBegin(int lineIndex)
        {
            if (_lineIndex == lineIndex)
            {
                _textShowEnd = false;
            }
        }

        private void OnTextShowEnd(int lineIndex)
        {
            if (_lineIndex == lineIndex)
            {
                _textShowEnd = true;
            }
        }
        
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);
            Debug.Log("Populate");

            if (_textShowEnd == false || Application.isPlaying == false)
            {
                return;
            }
            _textShowEnd = true;
            
             // 清空空格和换行的字符串
            _cleanText = CleanString(text, out List<int> indexMap);
            // 文本更新和渲染网格不同步
            if (_cleanText.Length * 4 != toFill.currentVertCount)
            {
                return;
            }
            _keywordBoxMap.Clear();

            List<UIVertex> vertexes = new List<UIVertex>(toFill.currentVertCount);
            for (int i = 0; i < toFill.currentVertCount; i++)
            {
                UIVertex tempVertex = new UIVertex();
                toFill.PopulateUIVertex(ref tempVertex, i);
                vertexes.Add(tempVertex);
            }

            List<int> lineStart = new List<int>(cachedTextGenerator.lineCount);
            for (int i = 0; i < cachedTextGenerator.lineCount; i++)
            {
                if (cachedTextGenerator.lines[i].startCharIdx < text.Length)
                {
                    lineStart.Add(cachedTextGenerator.lines[i].startCharIdx);
                }
            }
            lineStart.Add(int.MaxValue);

            // 所有关键词的起始和结束下标
            List<Range> keywordLocation = _keywordMatcher.Match(_cleanText);

            // 生成包围盒
            int nextLine = 1;
            foreach (Range t in keywordLocation)
            {
                int start = t.Left;
                int end = t.Right;
                int curStart = start;

                // Debug.Log("L: " + start + " R: " + end + " " + dueText.Substring(start, end - start + 1));
                while (lineStart[nextLine] <= indexMap[start])
                {
                    nextLine++;
                }

                string curKeyword = _cleanText.Substring(start, end - start + 1);

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

            if (_keywordBoxMap.TryGetValue(keyword, out List<BoundingBox> list))
            {
                list.Add(curBox);
            }
            else
            {
                _keywordBoxMap.Add(keyword, new List<BoundingBox>{curBox});
            }
        }

        private static string CleanString(string origin, out List<int> indexMap)
        {
            StringBuilder sb = new StringBuilder(origin.Length);
            List<int> indexes = new List<int>(origin.Length);

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
            foreach (KeyValuePair<string, List<BoundingBox>> pair in _keywordBoxMap)
            {
                foreach (BoundingBox boundingBox in pair.Value)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireCube(boundingBox.Center, boundingBox.Size);
                    BoundingBox box = new BoundingBox(boundingBox.Center, 
                        boundingBox.Size, 1 + _keywordConfigSO.ClickBoxExpandRatio);
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(box.Center, box.Size);
                }
            }
        }
    }
}