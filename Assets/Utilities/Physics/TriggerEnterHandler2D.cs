using UnityEngine;

namespace Utilities.Physics
{
    /// <summary>
    /// 触发器进入2D
    /// </summary>
    public class TriggerEnterHandler2D : MonoBehaviour
    {
        /// <summary> 外部事件 </summary>
        [SerializeField] private HandlerEvent _enterEvent;

        /// <summary> 层（可多选） </summary>
        [SerializeField] private LayerMask _layers;

        /// <summary> 触发器进入时调用 </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((1 << other.gameObject.layer & _layers) != 0)
            {
                _enterEvent.Invoke(other.gameObject);
            }
        }
    }
}
