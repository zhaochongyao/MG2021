using UnityEngine;

namespace Utilities.Physics
{
    /// <summary>
    /// 进入碰撞2D
    /// </summary>
    public class CollisionEnterHandler2D : MonoBehaviour
    {
        /// <summary> 外部事件 </summary>
        [SerializeField] private HandlerEvent _enterEvent;

        /// <summary> 层（可多选） </summary>
        [SerializeField] private LayerMask _layers;

        /// <summary> 碰撞进入时调用 </summary>
        private void OnCollisionEnter2D(Collision2D other)
        {
            if ((1 << other.gameObject.layer & _layers) != 0)
            {
                _enterEvent.Invoke(other.gameObject);
            }
        }
    }
}
