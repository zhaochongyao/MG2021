namespace Utilities.DesignPatterns
{
    /// <summary>
    /// 命令栈
    /// </summary>
    public sealed class CommandStack
    {
        /// <summary> 容器 </summary>
        private ICommand[] _commands;

        /// <summary> 指令位置 </summary>
        public int Current { get; private set; }

        /// <summary> 指令个数 </summary>
        public int Count { get; private set; }

        /// <summary> 容量 </summary>
        public int Capacity { get; private set; }

        /// <summary> 构造 </summary>
        public CommandStack(int capacity = 10)
        {
            Capacity = capacity;
            _commands = new ICommand[Capacity];
            Current = 0;
            Count = 0;
        }

        /// <summary> 调整大小 </summary>
        public void Resize(int newSize)
        {
            if (newSize == Capacity)
            {
                return;
            }

            ICommand[] newCopy = new ICommand[newSize];
            int len;
            if (Count < newSize)
            {
                len = Count;
            }
            else
            {
                len = newSize;
                Count = newSize;
            }

            for (int i = 0; i < len; ++i)
            {
                newCopy[i] = _commands[i];
            }

            _commands = newCopy;
            Capacity = newSize;
        }

        /// <summary> 缩小到合适大小 </summary>
        public void TrimExcess()
        {
            Resize(Count);
        }

        /// <summary> 执行指令并记录 </summary>
        public void Do(ICommand exec)
        {
            if (Current == Capacity)
            {
                Resize(2 * Capacity);
            }

            exec.Execute();
            _commands[Current] = exec;
            ++Current;
            Count = Current;
        }

        /// <summary> 撤销 </summary>
        public void Undo()
        {
            if (Current > 0)
            {
                --Current;
                _commands[Current].Revoke();
            }
        }

        /// <summary> 恢复 </summary>
        public void Redo()
        {
            if (Current < Count)
            {
                _commands[Current].Execute();
                ++Current;
            }
        }

    }
}
