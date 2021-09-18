using System.Collections.Generic;
using System.Text;

namespace Utilities.DataStructures
{
    /// <summary>
    /// 字典树（区分大小写）
    /// </summary>
    public sealed class Trie
    {
        /// <summary> 根节点 </summary>
        private readonly TrieNode _root;

        /// <summary> 初始化根节点 </summary>
        public Trie()
        {
            _root = new TrieNode();
        }

        /// <summary> 添加新单词 </summary>
        public void Add(string word)
        {
            TrieNode p = _root;
            foreach (char ch in word)
            {
                if (p.Children.TryGetValue(ch, out TrieNode child) == false)
                {
                    child = new TrieNode();
                    p.Children.Add(ch, child);
                }
                p = child;
            }
            p.EndMark = true;
        }

        /// <summary> 搜索单词，返回末尾结点引用 </summary>
        private TrieNode Search(string word)
        {
            TrieNode p = _root;
            foreach (char ch in word)
            {
                if (p.Children.TryGetValue(ch, out TrieNode child))
                {
                    p = child;
                }
                else
                {
                    return null;
                }
            }
            return p;
        }

        /// <summary> 是否包含单词 </summary>
        public bool Contains(string word)
        {
            TrieNode res = Search(word);
            if (res == null || res.EndMark == false)
            {
                return false;
            }

            return true;
        }

        /// <summary> 是否有包含该前缀的单词 </summary>
        public List<string> StartsWith(string prefix)
        {
            List<string> res = new List<string>();
            TrieNode head = Search(prefix);
            if (head == null)
            {
                return res;
            }

            // 预估字符串长度
            const int baseLength = 20;
            // 广搜
            StringBuilder builder = new StringBuilder(baseLength); // 只对后缀部分进行字符操作
            StringBuilder result = new StringBuilder(baseLength); // 存放前缀，与后缀连接后加入结果
            result.Append(prefix);

            Queue<QNode> queue = new Queue<QNode>();
            QNode p = new QNode(head, string.Empty);
            queue.Enqueue(p);
            while (queue.Count > 0)
            {
                p = queue.Dequeue();
                if (p.Current.EndMark)
                {
                    result.Append(p.Match);
                    // ToString无拷贝开销
                    res.Add(result.ToString());
                    result.Length = prefix.Length;
                }

                builder.Clear();
                builder.Append(p.Match);
                foreach(KeyValuePair<char, TrieNode> t in p.Current.Children)
                {
                    builder.Append(t.Key);
                    queue.Enqueue(new QNode(t.Value, builder.ToString()));
                    --builder.Length;
                }
            }

            return res;
        }

        /// <summary>
        /// 广搜结点
        /// </summary>
        private readonly struct QNode
        {
            public readonly TrieNode Current;

            public readonly string Match;

            public QNode(TrieNode current, string match)
            {
                Current = current;
                Match = match;
            }
        }

        /// <summary>
        /// 结点类
        /// </summary>
        private sealed class TrieNode
        {
            /// <summary> 子结点引用（哈希存放） </summary>
            public readonly Dictionary<char, TrieNode> Children;

            /// <summary> 结尾标记 </summary>
            public bool EndMark;

            /// <summary> 默认构造 </summary>
            public TrieNode()
            {
                Children = new Dictionary<char, TrieNode>();
                EndMark = false;
            }
        }
    }
}
