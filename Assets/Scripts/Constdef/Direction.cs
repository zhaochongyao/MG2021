using UnityEngine;

namespace Constdef
{
    public enum MoveDirection
    {
        Stay  = 0,
        Left = 1,
        Right = 2,
        Up = 3,
        Down = 4,
    
    }
    public static class Direction
    {
        public static Vector3 GetDirectionVector(MoveDirection direction)
        {
            switch (direction)
            {
                case MoveDirection.Left:
                {
                    return new Vector3(-1, 0, 0);
                }
                case MoveDirection.Right:
                {
                    return new Vector3(1, 0, 0);
                }
                case MoveDirection.Up:
                {
                    return new Vector3(0, 0, 1);
                }
                case MoveDirection.Down:
                {
                    return new Vector3(0, 0, -1);
                }
                case MoveDirection.Stay:
                {
                    break;
                }
            }
            return Vector3.zero;
        }
    }
}