using System;
using System.Collections.Generic;

namespace Utilities.AI
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    public sealed class StateMachine<T>
    {
        /// <summary> 当前状态 </summary>
        private State<T> _curState;

        /// <summary> 所有状态对应的过渡 </summary>
        private readonly Dictionary<State<T>, List<Transition>> _transitionGraph;

        /// <summary> 对任何状态都有效过渡 </summary>
        private readonly List<Transition> _anyTransitions;

        /// <summary> 状态机是否运行 </summary>
        public bool Running { get; private set; }
        
        /// <summary> 构造 </summary>
        public StateMachine()
        {
            _transitionGraph = new Dictionary<State<T>, List<Transition>>();
            _anyTransitions = new List<Transition>();
            Running = false;
        }

        /// <summary> 启动状态机 </summary>
        public void Boot()
        {
            _curState = DefaultState;
            _curState.OnEnter();
            Resume();
        }

        /// <summary> 暂停状态机 </summary>
        public void Stop()
        {
            if (Running == false)
            {
                return;
            }
            Running = false;
            ManagerProxy.Instance.UpdateEvent -= OnUpdate;
            ManagerProxy.Instance.FixedUpdateEvent -= OnFixedUpdate;
        }

        /// <summary> 继续状态机 </summary>
        public void Resume()
        {
            if (Running)
            {
                return;
            }
            Running = true;
            ManagerProxy.Instance.UpdateEvent += OnUpdate;
            ManagerProxy.Instance.FixedUpdateEvent += OnFixedUpdate;
        }

        /// <summary> 当前状态 </summary>
        public State<T> CurState => _curState;

        /// <summary> 默认状态 </summary>
        public State<T> DefaultState { get; set; }

        /// <summary> 用户需每帧更新状态机 </summary>
        private void OnUpdate()
        {
            State<T> next = null;
            
            // 任意过渡优先级高
            foreach (Transition transition in _anyTransitions)
            {
                if (transition.Condition())
                {
                    next = transition.To;
                    break;
                }
            }

            // 若next状态结点不存在任何指向外部的过渡边，跳过
            if (next == null && _transitionGraph.TryGetValue(_curState, out List<Transition> transitions))
            {
                foreach (Transition transition in transitions)
                {
                    if (transition.Condition())
                    {
                        next = transition.To;
                        break;
                    }
                }
            }

            // 很有可能Any过渡会多次满足条件进入同一状态，比较next状态与cur状态
            if (next == null || _curState == next)
            {
                _curState.OnUpdate();
            } 
            // 切换状态
            else
            {
                _curState.OnExit();
                _curState = next;
                next.OnEnter();
            }
        }

        /// <summary> 用户需每物理帧更新状态机 </summary>
        private void OnFixedUpdate()
        {
            _curState.OnFixedUpdate();
        }

        /// <summary> 添加过渡 </summary>
        public void AddTransition(State<T> from, State<T> to, Func<bool> condition)
        {
            Transition transition = new Transition(to, condition);
            if (_transitionGraph.TryGetValue(from, out List<Transition> transitions))
            {
                transitions.Add(transition);
            }
            else
            {
                _transitionGraph.Add(from, new List<Transition>{transition});                
            }
        }

        /// <summary> 添加任意过渡 </summary>
        public void AddAnyTransition(State<T> to, Func<bool> condition)
        {
            _anyTransitions.Add(new Transition(to, condition));
        }

        /// <summary>
        /// 过渡：状态机的一条单向边：存放到达的状态和条件
        /// </summary>
        private sealed class Transition
        {
            /// <summary> 目的状态 </summary>
            public readonly State<T> To;

            /// <summary> 条件 </summary>
            public readonly Func<bool> Condition;

            /// <summary> 构造 </summary>
            public Transition(State<T> to, Func<bool> condition)
            {
                To = to;
                Condition = condition;
            }
        }
    }
}
