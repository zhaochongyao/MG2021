using UnityEngine;

namespace KeywordSystem
{
    /// <summary>
    /// 包围盒结构体
    /// </summary>
    public readonly struct BoundingBox
    {
        private readonly Vector2 _min;

        private readonly Vector2 _max;

        /// <summary> 左下角 </summary>
        public Vector2 Min => _min;
        
        /// <summary> 右上角 </summary>
        public Vector2 Max => _max;

        /// <summary> 中心点 </summary>
        public Vector2 Center => new Vector2
        {
            x = (_min.x + _max.x) / 2,
            y = (_min.y + _max.y) / 2
        };

        /// <summary> 长宽 </summary>
        public Vector2 Size => new Vector2
        {
            x = _max.x - _min.x,
            y = _max.y - _min.y
        };

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="min"> 左下角 </param>
        /// <param name="max"> 右上角 </param>
        public BoundingBox(Vector2 min, Vector2 max)
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        /// 按照比例构造
        /// </summary>
        /// <param name="center"> 中心 </param>
        /// <param name="size"> 长宽 </param>
        /// <param name="ratio"> 放大或缩小比例 </param>
        public BoundingBox(Vector2 center, Vector2 size, float ratio)
        {
            size *= ratio;
            _min = new Vector2
            {
                x = center.x - size.x / 2,
                y = center.y - size.y / 2
            };
            _max = new Vector2
            {
                x = center.x + size.x / 2,
                y = center.y + size.y / 2
            };
        }

        /// <summary>
        /// 是否包含点
        /// </summary>
        /// <param name="point"> 点坐标 </param>
        public bool Contains(Vector2 point)
        {
            return _min.x <= point.x && point.x <= _max.x &&
                   _min.y <= point.y && point.y <= _max.y;
        }
    }
}