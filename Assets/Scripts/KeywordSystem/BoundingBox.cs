using UnityEngine;

namespace KeywordSystem
{
    public readonly struct BoundingBox
    {
        private readonly Vector2 _min;

        private readonly Vector2 _max;

        public Vector2 Min => _min;
        public Vector2 Max => _max;

        public Vector2 Center => new Vector2
        {
            x = (_min.x + _max.x) / 2,
            y = (_min.y + _max.y) / 2
        };

        public Vector2 Size => new Vector2
        {
            x = _max.x - _min.x,
            y = _max.y - _min.y
        };

        public BoundingBox(Vector2 min, Vector2 max)
        {
            _min = min;
            _max = max;
        }

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

        public bool Contains(Vector2 point)
        {
            return _min.x <= point.x && point.x <= _max.x &&
                   _min.y <= point.y && point.y <= _max.y;
        }
    }
}