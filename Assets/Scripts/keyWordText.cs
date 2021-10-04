// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;
//
// namespace KeywordComponent
// {
//     public delegate void TextClickEvent(string str);
//
//     public delegate float CompareFunc(float a, float b);
//
//     internal readonly struct Box
//     {
//         public readonly float MinX;
//         public readonly float MinY;
//         public readonly float MaxX;
//         public readonly float MaxY;
//
//         public Box(float x, float y, float xx, float yy)
//         {
//             MinX = x;
//             MinY = y;
//             MaxX = xx;
//             MaxY = yy;
//         }
//     }
//
//     public class KeywordText : Text, IPointerClickHandler
//     {
//         private readonly Dictionary<string, List<Box>> _keywordBox = new Dictionary<string, List<Box>>();
//
//         public readonly TextClickEvent KeywordClickEvent = delegate { };
//
//         public void OnPointerClick(PointerEventData eventData)
//         {
//             // 转换鼠标点击位置为矩形内坐标
//             RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position,
//                 eventData.pressEventCamera, out Vector2 localPosition);
//             
//             // 遍历包围盒，判断鼠标位置是否被包含
//             foreach (var boxList in _keywordBox)
//             {
//                 foreach (Box box in boxList.Value)
//                 {
//                     if (localPosition.x <= box.MaxX && localPosition.x >= box.MinX && localPosition.y <= box.MaxY &&
//                         localPosition.y >= box.MinY)
//                     {
//                         Debug.Log("点到了" + boxList.Key);
//
//                         if (KeywordClickEvent != null) KeywordClickEvent.Invoke(boxList.Key);
//                         return;
//                     }
//                 }
//             }
//
//             Debug.Log("点到了非关键词区域");
//         }
//
//         protected override void OnPopulateMesh(VertexHelper toFill)
//         {
//             base.OnPopulateMesh(toFill);
//             if (text.Length == 0) return;
//             _keywordBox.Clear();
//             List<int> lineStart = new List<int>();
//             List<int> indexMap = new List<int>();
//             
//             // 清空空格和换行的字符串
//             string dueText = DueString(text, ref indexMap);
//
//             for (int i = 0; i < cachedTextGenerator.lineCount; i++)
//             {
//                 if (cachedTextGenerator.lines[i].startCharIdx < text.Length)
//                 {
//                     lineStart.Add(cachedTextGenerator.lines[i].startCharIdx);
//                 }
//             }
//
//             lineStart.Add(int.MaxValue);
//             List<UIVertex> vertexes = new List<UIVertex>(toFill.currentVertCount);
//             for (int i = 0; i < toFill.currentVertCount; i++)
//             {
//                 UIVertex tempVertex = new UIVertex();
//                 toFill.PopulateUIVertex(ref tempVertex, i);
//                 vertexes.Add(tempVertex);
//             }
//
//             // 所有关键词的起始和结束下标
//             List<KeyValuePair<int, int>> keyWordLocation = Utils.GetInstance().MatchKeyWorld(dueText);
//
//             // 生成包围盒
//             int nextLine = 1;
//             foreach (KeyValuePair<int, int> t in keyWordLocation)
//             {
//                 int startChar = t.Key;
//                 int endChar = t.Value;
//                 int curStart = startChar;
//                 while (lineStart[nextLine] <= indexMap[startChar]) nextLine++;
//                 string curKeyword = dueText.Substring(startChar, endChar - startChar + 1);
//
//                 for (int charIndex = startChar; charIndex <= endChar; charIndex++)
//                 {
//                     if (charIndex < endChar && indexMap[charIndex + 1] >= lineStart[nextLine])
//                     {
//                         SetClickBox(vertexes, curStart, charIndex, toFill, curKeyword);
//                         curStart = charIndex + 1;
//                         nextLine++;
//                     }
//                 }
//
//                 SetClickBox(vertexes, curStart, endChar, toFill, curKeyword);
//             }
//         }
//
//         private void SetClickBox(List<UIVertex> vertexes, int startChar, int endChar, VertexHelper toFill,
//             string keyword)
//         {
//             if (endChar < startChar || (endChar + 1) * 4 > vertexes.Count)
//             {
//                 Debug.LogWarning("关键词超出rectTransform大小范围");
//                 return;
//             }
//
//             float minY = FindSuitableY(vertexes, startChar * 4, endChar * 4 + 3, (a, b) => a < b ? a : b);
//             float maxY = FindSuitableY(vertexes, startChar * 4, endChar * 4 + 3, (a, b) => a > b ? a : b);
//             float startX = vertexes[startChar * 4].position.x;
//             float endX = vertexes[endChar * 4 + 2].position.x;
//             Box curBox = new Box(startX, minY, endX, maxY);
//             if (_keywordBox.ContainsKey(keyword))
//             {
//                 _keywordBox[keyword].Add(curBox);
//             }
//             else
//             {
//                 List<Box> boxList = new List<Box> {curBox};
//                 _keywordBox.Add(keyword, boxList);
//             }
//
//             DrawUnderLine(toFill, new Vector2(startX, minY), new Vector2(endX, minY));
//         }
//
//         private static string DueString(string originalText, ref List<int> indexMap)
//         {
//             string resultStr = "";
//             indexMap.Clear();
//             for (int i = 0; i < originalText.Length; i++)
//             {
//                 if (originalText[i] != ' ' && originalText[i] != '\n')
//                 {
//                     resultStr += originalText[i];
//                     indexMap.Add(i);
//                 }
//             }
//
//             return resultStr;
//         }
//
//         private void DrawUnderLine(VertexHelper toFill, Vector2 startPos, Vector2 endPos)
//         {
//             Vector2 extents = rectTransform.rect.size;
//             cachedTextGenerator.Populate("_", GetGenerationSettings(extents));
//
//             float lineHeight = 0;
//             IList<UIVertex> textVertex = cachedTextGenerator.verts;
//             for (int i = 1; i < 4; i++)
//             {
//                 lineHeight = Mathf.Max(lineHeight, Mathf.Abs(textVertex[i].position.y - textVertex[i - 1].position.y));
//             }
//
//             float supY = startPos.y;
//             float infY = supY - lineHeight / 1.5f;
//
//             List<Vector2> posList = new List<Vector2>()
//             {
//                 new Vector2(startPos.x, supY), new Vector2(endPos.x, supY), new Vector2(endPos.x, infY),
//                 new Vector2(startPos.x, infY)
//             };
//
//             Vector4 centerUV = Vector4.zero;
//             for (int i = 0; i < 4; i++)
//             {
//                 centerUV += textVertex[i].uv0;
//             }
//
//             centerUV /= 4;
//
//             UIVertex[] tempVertexes = new UIVertex[4];
//             for (int i = 0; i < 4; i++)
//             {
//                 tempVertexes[i] = textVertex[i];
//                 tempVertexes[i].uv0 = (centerUV + textVertex[i].uv0) / 2;
//                 tempVertexes[i].color = color;
//                 tempVertexes[i].position = posList[i];
//             }
//
//             toFill.AddUIVertexQuad(tempVertexes);
//         }
//
//         private static float FindSuitableY(List<UIVertex> vertexes, int startIndex, int endIndex, CompareFunc comp)
//         {
//             if (endIndex < startIndex || endIndex >= vertexes.Count)
//             {
//                 Debug.LogError("FindSuitableY传入参数错误 startIndex = " + startIndex + " endIndex = " + endIndex +
//                                "顶点数量vertexes.count = " + vertexes.Count);
//                 return 0;
//             }
//
//             // 找到关键字中最高顶点或最低顶点
//             float needY = vertexes[startIndex].position.y;
//             for (int i = startIndex + 1; i <= endIndex; i++)
//             {
//                 needY = comp(needY, vertexes[i].position.y);
//             }
//
//             return needY;
//         }
//     }
// }