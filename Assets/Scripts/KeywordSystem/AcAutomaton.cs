using System.Collections.Generic;

namespace KeywordSystem
{
    public readonly struct Range
    {
        public readonly int Left;
        public readonly int Right;

        public Range(int left = 0, int right = 0)
        {
            Left = left;
            Right = right;
        }
    }
    
    /// <summary>
    /// 多模式串匹配
    /// （AC自动机实现）
    /// </summary>
    public sealed class AcAutomaton
    {
        /// <summary> 根节点 </summary>
        private Node _root;

        /// <summary> 初始化 </summary>
        public AcAutomaton()
        {
            _root = null;
        }

        /// <summary> 构建AC自动机 </summary>
        public void Construct(IEnumerable<string> words)
        {
            _root = new Node();
            // 构建字典树
            foreach (string word in words)
            {
                Add(word);
            }

            // 构建失配指针
            Build();
        }

        /// <summary> 添加新单词 </summary>
        private void Add(string word)
        {
            Node p = _root;
            foreach (char ch in word)
            {
                if (p.Children.TryGetValue(ch, out Node child) == false)
                {
                    child = new Node();
                    p.Children.Add(ch, child);
                }

                p = child;
            }

            // length > 0 也作为end标记
            p.Length = word.Length;
        }

        /// <summary> 构建FAIL指针 </summary>
        private void Build()
        {
            // 从根节点开始BFS
            Node cur = _root;
            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(cur);

            while (queue.Count > 0)
            {
                cur = queue.Dequeue();
                foreach (KeyValuePair<char, Node> t in cur.Children)
                {
                    // 对每个孩子结点
                    Node pChild = t.Value;
                    if (cur == _root)
                    {
                        // 若为根的直接子节点，fail指针直接指向根
                        pChild.Fail = _root;
                    }
                    else
                    {
                        char ch = t.Key;
                        // 子节点的fail指针初始和其父节点fail指针相同
                        Node f = cur.Fail;

                        while (f != null)
                        {
                            // 不断沿着fail链找和当前结点字符相同的结点，将当前结点fail指针指向他
                            if (f.Children.TryGetValue(ch, out Node fChild))
                            {
                                pChild.Fail = fChild;
                                break;
                            }
                            // 不断回溯fail指针
                            f = f.Fail;
                        }

                        // 若回溯到空fail指针也没有相匹配的结点，则将当前结点fail指针指向根
                        if (f == null)
                        {
                            pChild.Fail = _root;
                        }
                    }
                    queue.Enqueue(pChild);
                }
            }
        }

        public List<Range> Match(string text)
        {
            List<Range> res = new List<Range>();
            
            Node cur = _root;
            for (int i = 0; i < text.Length; ++i)
            {
                Node child;
                // 若失配，则不断回溯fail指针，继续匹配，直到回溯到根或匹配成功
                while (cur.Children.TryGetValue(text[i], out child) == false && cur != _root)
                {
                    cur = cur.Fail;
                }

                // 匹配不成功，从根重新开始匹配
                cur = child ?? _root;

                // 通过匹配到的尾结点存放的长度，可得到敏感词在输入串中的位置
                // 只有length大于0才会进行替换
                if (cur.Length != 0)
                {
                    res.Add(new Range(i - cur.Length + 1, i));
                }
            }
            return res;
        }

        /// <summary>
        /// 结点类
        /// </summary>
        private sealed class Node
        {
            /// <summary> 子结点引用（哈希存放） </summary>
            public readonly Dictionary<char, Node> Children;
            /// <summary> 已该结点为结尾的关键词长度（也作结尾标记） </summary>
            public int Length;
            /// <summary> 失配指针 </summary>
            public Node Fail;

            /// <summary> 默认构造 </summary>
            public Node()
            {
                Children = new Dictionary<char, Node>();
                Length = 0;
                Fail = null;
            }
        }
    }
}