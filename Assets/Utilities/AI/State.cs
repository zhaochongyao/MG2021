namespace Utilities.AI
{
    /// <summary>
    /// 状态
    /// </summary>
    public abstract class State<T>
    {
        protected readonly T _self;

        protected State(T owner)
        {
            _self = owner;
        }

        /// <summary> 状态进入时触发 </summary>
        public virtual void OnEnter()
        {
        }

        /// <summary> 状态保持时触发 </summary>
        public virtual void OnUpdate()
        {
        }
        
        /// <summary> 状态保持时触发 </summary>
        public virtual void OnFixedUpdate()
        {
        }

        /// <summary> 状态退出时触发 </summary>
        public virtual void OnExit()
        {
        }
    }
}