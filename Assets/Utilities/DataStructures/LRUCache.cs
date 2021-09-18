using System;
using System.Collections.Generic;

namespace Utilities.DataStructures
{
    /// <summary>
    /// 使用LRU置换策略的缓存
    /// 空间满，置换最久没用数据
    /// 场景：空间有限，只能放一部分的资源
    /// 且获取资源非常耗时，缓存一部分可以提高性能
    /// </summary>
    public sealed class LRUCache<TKey, TValue>
    {
        // 置换元素时，可能会需要写回操作
        // 缓存销毁时，全部元素都应当写回

        /// <summary> 最大容量 </summary>
        private int _capacity;

        /// <summary> 最大容量 </summary>
        public int Capacity 
        {
            get => _capacity;
            set
            {
                // 如果元素个数比新容量更大，则需要删除过多最久没用的元素
                if (Count > value)
                {
                    int get = Count - value;
                    for (int i = 0; i < get; ++i)
                    {
                        Node kick = _recent.RemoveRear();
                        _mapping.Remove(kick.Key);
                        // 写回
                        if (_writeBack != null)
                        {
                            _writeBack(kick.Value);
                        }
                    }
                }
                _capacity = value;
            }
        }

        /// <summary> 当前个数 </summary>
        public int Count => _mapping.Count;

        /// <summary> 键和双向链表结点的映射 </summary>
        private readonly Dictionary<TKey, Node> _mapping;

        /// <summary> 表示使用时间的远近的双向链表 </summary>
        private DoublyLinkedList _recent;

        /// <summary> 置换和销毁时的写回函数 </summary>
        private readonly Action<TValue> _writeBack;

        /// <summary> 初始化容量（如果需要，请传入写回函数） </summary>
        public LRUCache(int capacity, Action<TValue> writeBack = null)
        {
            // 不用传入回调函数则默认不需要写回
            _capacity = capacity;
            _mapping = new Dictionary<TKey, Node>();
            _recent = new DoublyLinkedList();
            _writeBack = writeBack;
        }

        /// <summary> 销毁时写回所有数据 </summary>
        ~LRUCache()
        {
            if (_writeBack != null)
            {
                foreach (Node node in _mapping.Values)
                {
                    _writeBack(node.Value);
                }
            }
        }

        /// <summary> 添加 </summary>
        public void Add(TKey key, TValue value)
        {
            if (_mapping.TryGetValue(key, out Node node))
            {
                node.Value = value;
                _recent.MoveFront(node);
            }
            else
            {
                if (_mapping.Count == _capacity)
                {
                    Node kick = _recent.RemoveRear();
                    _mapping.Remove(kick.Key);

                    // 写回置换元素
                    if (_writeBack != null)
                    {
                        _writeBack(kick.Value);
                    }
                }

                Node add = new Node(key, value);
                _recent.InsertFront(add);
                _mapping.Add(key, add);
            }
        }

        /// <summary> 是否包含 </summary>
        public bool Contains(TKey key)
        {
            return _mapping.ContainsKey(key);
        }

        /// <summary> 下标运算符 </summary>
        public TValue this[TKey key]
        {
            get
            {
                if (_mapping.TryGetValue(key, out Node node))
                {
                    _recent.MoveFront(node);
                    return node.Value;
                }
                return default;
            }
            set
            {
                if (_mapping.TryGetValue(key, out Node node))
                {
                    _recent.MoveFront(node);
                    node.Value = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        /// <summary> 尝试获取，返回包含与否 </summary>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_mapping.TryGetValue(key, out Node node))
            {
                _recent.MoveFront(node);
                value = node.Value;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary> 移除 </summary>
        public void Remove(TKey key)
        {
            if (_mapping.TryGetValue(key, out Node node))
            {
                _mapping.Remove(key);
                _recent.Remove(node);
            }
        }

        /// <summary> 清空 </summary>
        public void Clear()
        {
            _mapping.Clear();
            _recent = new DoublyLinkedList();
        }

        /// <summary>
        /// 双向链表
        /// </summary>
        private sealed class DoublyLinkedList
        {
            /// <summary> 虚拟头尾结点 </summary>
            private readonly Node _dummy;

            /// <summary> 构造 </summary>
            public DoublyLinkedList()
            {
                _dummy = new Node();

                _dummy.Next = _dummy;
                _dummy.Pre = _dummy;
            }

            /// <summary> 将指定结点移出 </summary>
            public void Remove(Node node)
            {
                node.Pre.Next = node.Next;
                node.Next.Pre = node.Pre;   
            }

            /// <summary> 将新申请结点插入到开头，表示最近使用了 </summary>
            public void InsertFront(Node node)
            {
                node.Next = _dummy.Next;
                node.Pre = _dummy;
                _dummy.Next.Pre = node;
                _dummy.Next = node;
            }

            /// <summary> 将内部结点移动到开头，表示最近使用了 </summary>
            public void MoveFront(Node node)
            {
                Remove(node);
                InsertFront(node);
            }

            /// <summary> 移出尾部结点，即移出最久没使用的结点 </summary>
            public Node RemoveRear()
            {
                Node rear = _dummy.Pre;
                Remove(rear);
                return rear;
            }
        }

        /// <summary>
        /// 双向链表结点
        /// </summary>
        private sealed class Node
        {
            public readonly TKey Key;

            public TValue Value;

            // 结点必须为class，使用struct会产生错误递归（值和引用的区别）
            public Node Pre;

            public Node Next;

            /// <summary> 空结点构造 </summary>
            public Node()
            {
                Key = default;
                Value = default;
                Pre = null;
                Next = null;
            }

            /// <summary> 数据结点构造 </summary>
            public Node(TKey key,  TValue value)
            {
                Key = key;
                Value = value;
                Pre = null;
                Next = null;
            }
        }
    }
}
