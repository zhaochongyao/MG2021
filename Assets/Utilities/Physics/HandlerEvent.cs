using UnityEngine;
using UnityEngine.Events;

namespace Utilities.Physics
{
    /// <summary>
    /// 可序列化的事件
    /// </summary>
    [System.Serializable]
    public class HandlerEvent : UnityEvent<GameObject>
    {

    }
}
