using UnityEngine;

namespace Utilities.Physics
{
    /// <summary>
    /// 触发器退出
    /// </summary>
    public class TriggerExitHandler : MonoBehaviour
    {
        /// <summary> 外部事件 </summary>
        [SerializeField] private HandlerEvent _exitEvent;

        /// <summary> 层（可多选） </summary>
        [SerializeField] private LayerMask _layers;
        
        /// <summary> 触发器退出时调用 </summary>
        private void OnTriggerStay(Collider other)
        {
            if ((1 << other.gameObject.layer & _layers) != 0)
            {
                _exitEvent.Invoke(other.gameObject);
            }
        }
    }
}