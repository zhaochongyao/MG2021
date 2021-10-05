using System.Collections.Generic;
using UnityEngine;

namespace KeywordSystem
{
    public class KeywordTester : MonoBehaviour
    {
        private AcAutomaton _acAutomaton;

        [SerializeField] private List<string> _keywords;

        [SerializeField, TextArea] private string _text;
        
        private static List<Range> MergeRange(List<Range> rangeList)
        {
            List<Range> res = new List<Range> {rangeList[0]};
            int i, cur = 0;
            for (i = 1; i < rangeList.Count; ++i)
            {
                if (res[cur].Right + 1 >= rangeList[i].Left)
                {
                    res[cur] = new Range(res[cur].Left, rangeList[i].Right);
                }
                else
                {
                    res.Add(rangeList[i]);
                    ++cur;
                }
            }

            return res;
        }
        
        private void Start()
        {
            _acAutomaton = new AcAutomaton();
            _acAutomaton.Construct(_keywords);
            var res = _acAutomaton.Match(_text);
            foreach (Range pair in res)
            {
                string key = "";
                for (int i = pair.Left; i <= pair.Right; ++i)
                {
                    key += _text[i];
                }
                Debug.Log(pair.Left + " " + pair.Right + " " + key);
            }
        }
    }
}