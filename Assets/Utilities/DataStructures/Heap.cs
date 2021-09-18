using System.Collections.Generic;

namespace Utilities.DataStructures
{
    /// <summary>
    /// 堆实现的优先队列
    /// </summary>
    public sealed class Heap<T> : IPriorityQueue<T>
    {
        /// <summary> 获取当前元素个数 </summary>
        public int Count => _heap.Count;

        /// <summary> 比较器 </summary>
        public IComparer<T> Comparer { get ; set ; }

        /// <summary> 堆 </summary>
        private readonly List<T> _heap;

        /// <summary> 无参，需传入比较器后使用 </summary>
        public Heap()
        {
            _heap = new List<T>();
        }

        /// <summary> 传入比较器 </summary>
        public Heap(IComparer<T> comparer) 
        {
            _heap = new List<T>();
            Comparer = comparer;
        }

        /// <summary> 入队列 </summary>
        public void Enqueue(T elem)
        {
            // 放入尾部
            _heap.Add(elem);

            // 与父节点比较大小，进行上浮
            int ch = _heap.Count - 1;
            int pa = (ch - 1) / 2;
            while (ch > 0 && Comparer.Compare(_heap[ch], _heap[pa]) < 0)
            { // 比较原则：左边严格小于右边
                T temp = _heap[ch];
                _heap[ch] = _heap[pa];
                _heap[pa] = temp;
    
                ch = pa;
                pa = (ch - 1) / 2;
            }
        }

        /// <summary> 出队列 </summary>
        public T Dequeue()
        {
            // 记录队头元素
            T ret = _heap[0];

            // 用队尾元素覆盖队头元素，并删去
            int end = _heap.Count - 1;
            _heap[0] = _heap[end];
            _heap.RemoveAt(end);

            // 与左右子节点比较，进行下降
            --end;
            int pa = 0;
            int ch = 1;
            while (ch <= end)
            {
                int rch = ch + 1;
                // 与比较右孩子
                if (rch <= end && Comparer.Compare(_heap[rch], _heap[ch]) < 0)
                {
                    ch = rch;
                }

                if (Comparer.Compare(_heap[ch], _heap[pa]) >= 0)
                {
                    break;
                }

                T temp = _heap[ch];
                _heap[ch] = _heap[pa];
                _heap[pa] = temp;

                pa = ch;
                ch = 2 * pa + 1;
            }

            return ret;
        }

        /// <summary> 队列头部元素 </summary>
        public T Front => _heap[0];

        /// <summary> 清空队列 </summary>
        public void Clear()
        {
            _heap.Clear();
        }
    }
}
