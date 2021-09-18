
namespace Utilities
{
    /// <summary>
    /// 异步操作句柄
    /// 对象池使用
    /// </summary>
    public sealed class AsyncHandle
    {
        /// <summary> 调用者的引用 </summary>
        private readonly ObjectPool _pool;

        /// <summary> 构造 </summary>
        public AsyncHandle(ObjectPool pool)
        {
            IsDone = false;
            _pool = pool;
            _pool.AsyncDoneEvent += OnAsyncDone;
        }

        /// <summary> 异步操作是否完成 </summary>
        public bool IsDone { get; private set; }

        /// <summary> 异步完成后 </summary>
        private void OnAsyncDone()
        {
            IsDone = true;
            _pool.AsyncDoneEvent -= OnAsyncDone;
        }
    }
}