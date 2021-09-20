using UnityEngine;

namespace UtilitiesExample
{
    /// <summary>
    /// 玩家控制器
    /// </summary>
    public class Player : MonoBehaviour
    {
        /// <summary> 速度 </summary>
        [Header("参数设置")] 
        [SerializeField] private float _speed;

        private Vector3 _move;
    
        /// <summary> 刚体 </summary>
        [Header("组件引用")]
        [SerializeField] private Rigidbody _body;

        /// <summary> 每渲染帧 </summary>
        private void Update()
        {
            _move.y = 0f;
            ArrowKeyInput();
        }

        /// <summary> 每物理帧 </summary>
        private void FixedUpdate()
        {
            Move();
        }

        /// <summary> 方向键输入 </summary>
        private void ArrowKeyInput()
        {
            _move.x = Input.GetAxisRaw("Horizontal");
            _move.z = Input.GetAxisRaw("Vertical");
        }

        /// <summary> 移动 </summary>
        private void Move()
        {
            _move.Normalize();
            _move.x *= _speed;
            _move.z *= _speed;
            _body.velocity = _move;
        }
    }
}