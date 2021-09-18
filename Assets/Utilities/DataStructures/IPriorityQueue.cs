using System.Collections.Generic;

namespace Utilities.DataStructures
{
    /// <summary>
    /// 优先队列接口
    /// </summary>
    public interface IPriorityQueue<T>
    {
        /// <summary> 获取当前元素个数 </summary>
        int Count { get; }

        /// <summary> 比较器 </summary>
        IComparer<T> Comparer { get; set; }

        /// <summary> 出队列 </summary>
        T Dequeue();

        /// <summary> 入队列 </summary>
        void Enqueue(T t);

        /// <summary> 队列头部元素 </summary>
        T Front { get; }

        /// <summary> 清空队列 </summary>
        void Clear();
    }
}

// 支持迭代器
// T 支持比较器
