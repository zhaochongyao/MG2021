using UnityEngine;

namespace Utilities.Physics
{
    /// <summary>
    /// 保持碰撞
    /// </summary>
    public class CollisionStayHandler : MonoBehaviour
    {
        /// <summary> 外部事件 </summary>
        [SerializeField] private HandlerEvent _enterEvent;

        /// <summary> 层（可多选） </summary>
        [SerializeField] private LayerMask _layers;

        /// <summary> 碰撞保持时调用 </summary>
        private void OnCollisionStay(Collision other)
        {
            if ((1 << other.gameObject.layer & _layers) != 0)
            {
                _enterEvent.Invoke(other.gameObject);
            }
        }
    }
}