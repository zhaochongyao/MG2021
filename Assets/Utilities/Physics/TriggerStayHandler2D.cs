using UnityEngine;

namespace Utilities.Physics
{
    /// <summary>
    /// 触发器停留2D
    /// </summary>
    public class TriggerStayHandler2D : MonoBehaviour
    {
        /// <summary> 外部事件 </summary>
        [SerializeField] private HandlerEvent _stayEvent;

        /// <summary> 层（可多选） </summary>
        [SerializeField] private LayerMask _layers;
        
        /// <summary> 触发器停留时调用 </summary>
        private void OnTriggerStay2D(Collider2D other)
        {
            if ((1 << other.gameObject.layer & _layers) != 0)
            {
                _stayEvent.Invoke(other.gameObject);
            }
        }
    }
}