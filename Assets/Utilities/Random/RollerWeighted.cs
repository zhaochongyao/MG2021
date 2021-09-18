using System.Collections.Generic;

namespace Utilities.Random
{
    /// <summary>
    /// 带权重的随机数产生器
    /// </summary>
    public sealed class RollerWeighted
    {
        /// <summary> 权重的前缀和 </summary>
        private float[] _preSum;

        /// <summary> 权重 </summary>
        private IList<float> _weight;

        /// <summary> 权重 </summary>
        public IList<float> Weight => _weight;

        /// <summary> 初始化 </summary>
        public RollerWeighted(IList<float> weight = null)
        {
            if (weight != null)
            {
                SetWeight(weight);
            }
        }

        /// <summary> 设置权重 </summary>
        public void SetWeight(IList<float> weight)
        {
            _weight = weight;
            _preSum = new float[weight.Count];
            // 对权重求前缀和
            _preSum[0] = weight[0];
            for (int i = 1; i < _preSum.Length; ++i)
            {
                _preSum[i] = _preSum[i - 1] + weight[i];
            }
        }

        /// <summary> 带权重的独立随机 </summary>
        public int Roll()
        {
            // 在0-总权重内随机一个数，用二分查找位于哪一段权重区间内
            float res = UnityEngine.Random.Range(0.0f, _preSum[_preSum.Length - 1]);

            int left = 0, right = _preSum.Length - 1;
            while (left <= right)
            {
                int mid = (left + right) / 2;
                if (res <= _preSum[mid])
                {
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }
            return left;
        }
    }
}
