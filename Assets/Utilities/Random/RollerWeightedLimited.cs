using System.Collections.Generic;

namespace Utilities.Random
{
    /// <summary>
    /// 有上限的带权随机数产生器
    /// </summary>
    public sealed class RollerWeightedLimited
    {
        /// <summary> 权重 </summary>
        private IList<float> _weight;

        /// <summary> 权重 </summary>
        public IList<float> Weight => _weight;
        
        /// <summary> 权重上限 </summary>
        public float Limit { get; private set; }

        /// <summary> 初始化 </summary>
        public RollerWeightedLimited(IList<float> weight = null, float limit = 1.0f)
        {
            if (weight != null)
            {
                SetWeight(weight);
            }
            Limit = limit;
        }

        /// <summary> 设置权重 </summary>
        public void SetWeight(IList<float> weight)
        {
            _weight = weight;
        }

        /// <summary> 随机 </summary>
        public int Roll()
        {
            // 若有某项的权重超过权重总合上限，则随机的结果为必得某项
            float res = UnityEngine.Random.Range(0.0f, Limit);
            float accu = 0;
            int i;
            for (i = 0; i < _weight.Count; ++i)
            {
                accu += _weight[i];
                // 位置越前越先被判断，优先级越高
                if (accu >= res)
                {
                    break;
                }
            }
            // 若总权重小于上限，则随机可能落空
            // -1表示没有随机到任何项，此时需对随机结果作判断
            return accu >= res ? i : -1;
        }
    }
}