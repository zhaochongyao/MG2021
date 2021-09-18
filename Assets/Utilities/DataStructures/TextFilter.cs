using System.Collections.Generic;
using System.Text;

namespace Utilities.DataStructures
{
    /// <summary>
    /// 敏感词过滤器
    /// （AC自动机实现）
    /// </summary>
    public sealed class TextFilter
    {
        /// <summary> 根节点 </summary>
        private FNode _root;

        /// <summary> 初始化 </summary>
        public TextFilter()
        {
            _root = null;
        }

        /// <summary> 构建AC自动机 </summary>
        public void Construct(IList<string> words)
        {
            _root = new FNode();
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
            FNode p = _root;
            foreach (char ch in word)
            {
                if (p.Children.TryGetValue(ch, out FNode child) == false)
                {
                    child = new FNode();
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
            FNode p = _root;
            Queue<FNode> queue = new Queue<FNode>();
            queue.Enqueue(p);
        
            while (queue.Count > 0)
            {
                p = queue.Dequeue();
                foreach(KeyValuePair<char, FNode> t in p.Children)
                {
                    // 对每个孩子结点
                    FNode pChild = t.Value;
                    if (p == _root)
                    {
                        // 若为根的直接子节点，fail指针直接指向根
                        pChild.Fail = _root;
                    }
                    else
                    {
                        char ch = t.Key;

                        // 子节点的fail指针初始和其父节点fail指针相同
                        FNode f = p.Fail;

                        while (f != null)
                        {
                            // 不断沿着fail链找和当前结点字符相同的结点，将当前结点fail指针指向他
                            if (f.Children.TryGetValue(ch, out FNode fChild))
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

        /// <summary> 替换文本中的敏感词 </summary>
        public string Filter(string text, char replace = '*')
        {
            StringBuilder result = new StringBuilder(text);
            FNode p = _root;
            for (int i = 0; i < text.Length; ++i)
            {
                FNode child;
                // 若失配，则不断回溯fail指针，继续匹配，直到回溯到根或匹配成功
                while (p.Children.TryGetValue(text[i], out child) == false && p != _root)
                {
                    p = p.Fail;
                }

                p = child;
                if (p == null)
                {
                    // 匹配不成功，从根重新开始匹配
                    p = _root;
                }

                // 通过匹配到的尾结点存放的长度，可得到敏感词在输入串中的位置
                // 只有length大于0才会进行替换
                for (int j = i - p.Length + 1; j <= i; ++j)
                {
                    result[j] = replace;
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// 结点类
        /// </summary>
        private sealed class FNode
        {
            /// <summary> 子结点引用（哈希存放） </summary>
            public readonly Dictionary<char, FNode> Children;

            /// <summary> 已该结点为结尾的关键词长度（也作结尾标记） </summary>
            public int Length;

            /// <summary> 失配指针 </summary>
            public FNode Fail;

            /// <summary> 默认构造 </summary>
            public FNode()
            {
                Children = new Dictionary<char, FNode>();
                Length = 0;
                Fail = null;
            }
        }
    }
}
