using System.Collections.Generic;
using UnityEngine;
using Utilities.DataStructures;

namespace Utilities.Random
{
    /// <summary>
    /// 静态随机拓展类
    /// </summary>
    public static class RandomEx
    {
        /// <summary> 误差 </summary>
        private static float _epsilon = 0.001f;

        /// <summary> 误差 </summary>
        public static float Epsilon 
        {
            get => _epsilon;
            set
            {
                _epsilon = value;
                _cache.Clear();
            }
        }

        /// <summary> 最大迭代次数 </summary>
        private static int _maxCal = 20;

        /// <summary> 最大迭代次数 </summary>
        public static int MaxCal
        {
            get => _maxCal;
            set
            {
                _maxCal = value;
                _cache.Clear();
            }
        }

        /// <summary> PRD系数缓存容量 </summary>
        public static int Capacity
        {
            get => _cache.Capacity;
            set => _cache.Capacity = value;
        }

        /// <summary> 获取伪随机分布C系数 </summary>
        public static float GetPRD(float possibility)
        {
            // 在缓存取之，不在则算之（算毕，入缓存）
            if (_cache.TryGetValue(possibility, out float count))
            {
                return count;
            }
            float res = BinaryPRD(possibility);
            _cache.Add(possibility, count);
            return res;
        }

        /// <summary> PRD系数缓存 </summary>
        private static readonly LFUCache<float, float> _cache = new LFUCache<float, float>(20);
        // 默认容量为20

        /// <summary> 是否逼近 </summary>
        private static bool Near(float lhs, float rhs)
        {
            return Mathf.Abs(lhs - rhs) < _epsilon;
        }

        /// <summary> Count单次期望计算 </summary>
        private static float CountMean(float cnt)
        {
            // 0到1的小数，方便计算
            float expectation = 0.0f;
            float preFail = 1.0f;
            int maxFail = Mathf.CeilToInt(1.0f/ cnt);
            for (int i = 1; i <= maxFail; ++i)
            {
                // 第i次独立触发概率
                float cur = Mathf.Min(1.0f, i * cnt);
                expectation += i * preFail * cur;
                // 之前几次全部失败的概率
                preFail *= 1.0f - cur;
            }
            // 期望触发次数的倒数即为单次触发的期望概率
            return 1.0f / expectation;
        }

        /// <summary> 二分寻找最佳Count </summary>
        private static float BinaryPRD(float possibility)
        {
            float left = 0.0f, right = possibility, midCnt;
            int time = 0;
            while (true)
            {
                midCnt = (left + right) / 2.0f;
                float expectation = CountMean(midCnt);

                // 单次触发期望接近标准概率时，或计算次数超过限制时，退出二分
                if (Near(expectation, possibility) || time > _maxCal)
                {
                    break;
                }

                if (expectation < possibility)
                {
                    left = midCnt;
                }
                else
                {
                    right = midCnt;
                }
                ++time;
            }
            return midCnt;
        }

        /// <summary> 独立随机（概率为0.0-1.0之间） </summary>
        public static bool Roll(float possibility)
        {
            return UnityEngine.Random.Range(0.0f, 1.0f) <= possibility;
        }

        // IList对数组和List都通用
        /// <summary> 打乱顺序 </summary>
        public static void Shuffle<T>(IList<T> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                int choose = UnityEngine.Random.Range(i, list.Count);
                T temp = list[i];
                list[i] = list[choose];
                list[choose] = temp;
            }
        }

        /// <summary> 随机选择一个 </summary>
        public static T Choose<T>(IList<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
    }
}
