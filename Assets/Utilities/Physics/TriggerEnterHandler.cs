using UnityEngine;

namespace Utilities.Physics
{
    // 当父物体需要多个触发器且对各个触发器有对应的事件函数
    // 应将该脚本挂载在有相应触发器的子物体上，实现多个触发器的事件监听
    /// <summary>
    /// 触发器进入
    /// </summary>
    public class TriggerEnterHandler : MonoBehaviour
    {
        /// <summary> 外部事件 </summary>
        [SerializeField] private HandlerEvent _enterEvent;

        /// <summary> 层（可多选） </summary>
        [SerializeField] private LayerMask _layers;

        /// <summary> 触发器进入时调用 </summary>
        private void OnTriggerEnter(Collider other)
        {
            if ((1 << other.gameObject.layer & _layers) != 0)
            {
                _enterEvent.Invoke(other.gameObject);
            }
        }
    }
}
