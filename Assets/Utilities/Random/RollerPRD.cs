namespace Utilities.Random
{
    /// <summary>
    /// 伪随机分布数产生器
    /// </summary>
    public class RollerPRD
    {
        /// <summary> 当前计数 </summary>
        public int Count { get; private set; }

        /// <summary> 概率增量 </summary>
        private float _possIncrement;

        /// <summary> 概率增量 </summary>
        public float PossIncrement 
        {
            get => _possIncrement;
            set
            {
                _possIncrement = value;
                Count = 1;
            }
        }

        /// <summary> 初始化 </summary>
        public RollerPRD(float possibility)
        {
            SetPossibility(possibility);
        }

        /// <summary> 设置概率 </summary>
        public void SetPossibility(float possibility)
        {
            Count = 1;
            _possIncrement = RandomEx.GetPRD(possibility);
        }

        /// <summary> 产生随机结果 </summary>
        public bool Roll()
        {
            bool flag = RandomEx.Roll(Count * _possIncrement);
            // 若成功，count重置，否则+1
            Count = flag ? 1 : Count + 1;

            return flag;
        }

    }
}
