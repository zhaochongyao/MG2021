using UnityEngine;

namespace Utilities.Physics
{
    /// <summary>
    /// 退出碰撞
    /// </summary>
    public class CollisionExitHandler : MonoBehaviour
    {
        /// <summary> 外部事件 </summary>
        [SerializeField] private HandlerEvent _enterEvent;

        /// <summary> 层（可多选） </summary>
        [SerializeField] private LayerMask _layers;

        /// <summary> 碰撞退出时调用 </summary>
        private void OnCollisionExit(Collision other)
        {
            if ((1 << other.gameObject.layer & _layers) != 0)
            {
                _enterEvent.Invoke(other.gameObject);
            }
        }
    }
}
