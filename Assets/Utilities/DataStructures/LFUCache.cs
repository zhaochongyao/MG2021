using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.DataStructures
{
    /// <summary>
    /// 使用LFU置换策略的缓存
    /// 空间满，置换使用频率最低且最久没使用的数据（如有多个数据使用频率都是最低）
    /// </summary>
    public sealed class LFUCache<TKey, TValue>
    {
        /// <summary> 最大容量 </summary>
        private int _capacity;

        /// <summary> 最大容量（强烈不建议将新容量设置得比当前元素个数还小，会引发大量性能消耗） </summary>
        public int Capacity 
        { 
            get => _capacity;
            set
            {
                if (Count > value)
                {
                    int get = Count - value;
                    for (int i = 0; i < get; ++i)
                    {
                        Node kick = _freqList[_minFreq].RemoveRear();
                        _mapping.Remove(kick.Key);

                        // 写回
                        if (_writeBack != null)
                        {
                            _writeBack(kick.Value);
                        }
                    
                        // 去除空链表，并遍历寻找新的最小使用频率（消耗性能）
                        if (_freqList[_minFreq].Empty())
                        {
                            _freqList.Remove(_minFreq);
                            _minFreq = int.MaxValue;
                            foreach (int freq in _freqList.Keys.Where(freq => freq < _minFreq))
                            {
                                _minFreq = freq;
                            }
                        }
                    }
                }
                _capacity = value;
            }
        }

        /// <summary> 当前个数 </summary>
        public int Count => _mapping.Count;

        /// <summary> 最低的使用频率 </summary>
        private int _minFreq;

        /// <summary> 键和所有结点的映射 </summary>
        private readonly Dictionary<TKey, Node> _mapping;

        /// <summary> 
        /// 映射键值为频率和全部对应该频率的结点的集合
        /// 集合用双向链表表示，每个结点频率相同
        /// 同时按使用时间从近到远排列 
        /// </summary>
        private readonly Dictionary<int, DoublyLinkedList> _freqList;

        /// <summary> 置换和销毁时的写回函数 </summary>
        private readonly Action<TValue> _writeBack;

        /// <summary> 初始化容量（如果需要，请传入写回函数） </summary>
        public LFUCache(int capacity, Action<TValue> writeBack = null)
        {
            _capacity = capacity;
            _minFreq = 0;
            _mapping = new Dictionary<TKey, Node>();
            _freqList = new Dictionary<int, DoublyLinkedList>();

            _writeBack = writeBack;
        }

        /// <summary> 销毁时写回所有数据 </summary>
        ~LFUCache()
        {
            if (_writeBack != null)
            {
                foreach (Node node in _mapping.Values)
                {
                    _writeBack(node.Value);
                }
            }
        }

        /// <summary> 更新指定结点频率 </summary>
        private void UpdateFreq(Node node)
        {
            int freq = node.Freq;
            ++node.Freq;

            _freqList[freq].Remove(node);
            if (_freqList[freq].Empty())
            {
                _freqList.Remove(freq);
                if (_minFreq == freq)
                {
                    ++_minFreq;
                }
            }

            ++freq;
            if (_freqList.TryGetValue(freq, out DoublyLinkedList list))
            {
                list.InsertFront(node);
            }
            else
            {
                list = new DoublyLinkedList();
                list.InsertFront(node);
                _freqList.Add(freq, list);
            }
        }

        /// <summary> 重载下标运算符 </summary>
        public TValue this[TKey key]
        {
            get
            {
                if (_mapping.TryGetValue(key, out Node node))
                {
                    UpdateFreq(node);
                    return node.Value;
                }

                return default;
            }
            set
            {
                if (_mapping.TryGetValue(key, out Node node))
                {
                    UpdateFreq(node);
                    node.Value = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        /// <summary> 是否包含 </summary>
        public bool Contains(TKey key)
        {
            return _mapping.ContainsKey(key);
        }

        /// <summary> 添加 </summary>
        public void Add(TKey key, TValue value)
        {
            if (_mapping.TryGetValue(key, out Node node))
            {
                UpdateFreq(node);
                node.Value = value;
            }
            else
            {
                // 容量满，移除最不常用的结点
                if (_mapping.Count == _capacity)
                {
                    Node kick = _freqList[_minFreq].RemoveRear();
                    if (_freqList[_minFreq].Empty())
                    {
                        _freqList.Remove(_minFreq);     
                    }
                    _mapping.Remove(kick.Key);
                    // 写回置换元素
                    if (_writeBack != null)
                    {
                        _writeBack(kick.Value);
                    }
                }

                // 添加新的
                node = new Node(key, value);
                _mapping.Add(key, node);
                if (_freqList.TryGetValue(1, out DoublyLinkedList list))
                {
                    list.InsertFront(node);
                }
                else
                {
                    list = new DoublyLinkedList();
                    list.InsertFront(node);
                    _freqList.Add(1, list);
                }

                _minFreq = 1;
            }
        }

        /// <summary> 一次性判断存在性并返回数据 </summary>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_mapping.TryGetValue(key, out Node node))
            {
                value = node.Value;
                UpdateFreq(node);
                return true;
            }

            value = default;
            return false;
        }

        /// <summary> 移除 </summary>
        public void Remove(TKey key)
        {
            // 有键才能移除
            if (_mapping.TryGetValue(key, out Node node))
            {
                _mapping.Remove(key);
                int freq = node.Freq;
                _freqList[freq].Remove(node);
                // 移除空链表
                if (_freqList[freq].Empty())
                {
                    _freqList.Remove(freq);
                    // 若移除的为最低频率的链表
                    // 需遍历所有频率，找到最低的
                    // 此时时间复杂度较高
                    if (freq == _minFreq)
                    {
                        _minFreq = int.MaxValue;
                        foreach(int f in _freqList.Keys)
                        {
                            if (f < _minFreq)
                            {
                                _minFreq = f;
                            }
                        }
                    }
                }
            }
        }

        /// <summary> 清空 </summary>
        public void Clear()
        {
            _minFreq = 0;
            _mapping.Clear();
            _freqList.Clear();
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
            
            /// <summary> 判空 </summary>
            public bool Empty()
            {
                return _dummy.Next == _dummy;
            }
        }

        /// <summary>
        /// 双向链表结点
        /// </summary>
        private sealed class Node
        {
            /// <summary> 使用频率 </summary>
            public int Freq;

            public readonly TKey Key;

            public TValue Value;

            public Node Pre;

            public Node Next;

            /// <summary> 空结点构造 </summary>
            public Node()
            {
                Key = default;
                Value = default;
                Freq = 0;
                Pre = null;
                Next = null;
            }

            /// <summary> 数据结点构造 </summary>
            public Node(TKey key, TValue value)
            {
                Key = key;
                Value = value;
                Freq = 1;
                Pre = null;
                Next = null;
            }
        }
    }
}
