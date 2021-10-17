using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.DesignPatterns;

namespace Iphone
{
    [Serializable]
    public class DotReliance
    {
        [SerializeField] private GameObject _self;
        [SerializeField] private GameObject _next;

        public GameObject Self => _self;
        public GameObject Next => _next;
    }

    public class NodeData
    {
        public List<GameObject> NextList { get; }
        public int RefCnt { get; set; }

        public NodeData()
        {
            NextList = new List<GameObject>();
            RefCnt = 0;
        }
        
        public NodeData(GameObject go)
        {
            NextList = new List<GameObject> {go};
            RefCnt = 0;
        }
    }

    public class RedDotManager : LSingleton<RedDotManager>
    {
        [SerializeField] private List<DotReliance> _dotRelianceList;

        private Dictionary<GameObject, NodeData> _dotRelianceTree;

        protected override void Awake()
        {
            base.Awake();
            
            _dotRelianceTree = new Dictionary<GameObject, NodeData>();
            foreach (DotReliance dotReliance in _dotRelianceList)
            {
                AddReliance(dotReliance.Self, dotReliance.Next);
            }
        }

        public void AddReliance(GameObject self, GameObject next)
        {
            self.SetActive(false);
            next.SetActive(false);
            
            if (_dotRelianceTree.TryGetValue(self, out NodeData node))
            {
                node.NextList.Add(next);
            }
            else
            {
                _dotRelianceTree.Add(self, new NodeData(next));
            }
            
            if (_dotRelianceTree.ContainsKey(next) == false)
            {
                _dotRelianceTree.Add(next, new NodeData());
            }
        }

        /// <summary>
        /// 启动红点，及其父红点
        /// </summary>
        /// <param name="redDotObject"> 红点对象 </param>
        public void ShowRedDot(GameObject redDotObject)
        {
            BreadthFirstSearch(redDotObject, (go, node) =>
            {
                node.RefCnt++;
                go.SetActive(true);
            });
    }

        /// <summary>
        /// 启动红点，及其父红点
        /// </summary>
        /// <param name="redDotObject"> 红点对象 </param>
        public void HideRedDot(GameObject redDotObject)
        {
            BreadthFirstSearch(redDotObject, (go, node) =>
            {
#if UNITY_EDITOR
                if (node.RefCnt == 0)
                {
                    Debug.LogError("引用计数非法");
                    return;
                }
#endif
                node.RefCnt--;
                if (node.RefCnt == 0)
                {
                    go.SetActive(false);
                }
            });
        }

        private void BreadthFirstSearch(GameObject redDotObject, Action<GameObject, NodeData> operation)
        {
            Queue<GameObject> queue = new Queue<GameObject>();
            queue.Enqueue(redDotObject);
            while (queue.Count > 0)
            {
                GameObject cur = queue.Dequeue();

                NodeData node = _dotRelianceTree[cur];
                operation.Invoke(cur, node);

                foreach (GameObject nextRedDot in node.NextList)
                {
                    queue.Enqueue(nextRedDot);
                }
            }
        }
    }
}