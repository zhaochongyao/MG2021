using System;
using System.Collections;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    enum MoveDirection
    {
        Left = -1,
        Right = 1
    }
    public class player : MonoBehaviour
    {
        public bool jump = false;
        private Rigidbody2D _rigidbody;
        public Vector3 aimPoint;
        public float moveSpeed = 3f;
        private Collider2D _mayCollider;
        public float detact = 3f;
        private MoveDirection aimDirection;
        private void Start()
        {
            _rigidbody = gameObject.GetComponent<Rigidbody2D>();
            aimPoint = transform.position;
            EventCenter.GetInstance().AddEventListener<Vector3>("player_move_event", MoveTo);
        }

        private void MoveTo(Vector3 point)
        {
            jump = true;
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            point.z = screenPoint.z;
            aimPoint = Camera.main.ScreenToWorldPoint(point);
            MoveDirection direction = aimPoint.x > transform.position.x ? MoveDirection.Right : MoveDirection.Left;
            Vector2 hitDirection = new Vector2((float)direction, 0);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, hitDirection, 1);
            Debug.DrawLine(transform.position, transform.position+new Vector3(hitDirection.x, hitDirection.y, 0)*100, Color.red);
            if (hit.collider != null && hit.transform.position.x > point.x)
            {
                _mayCollider = hit.collider;
                Debug.Log(hit.transform.name);
            }
            if (transform.position.x > aimPoint.x)
            {
                aimDirection = MoveDirection.Left;
            }
            
            if (transform.position.x < aimPoint.x)
            {
                aimDirection = MoveDirection.Right;
            }
        }

        private void Jump()
        {
            jump = false;
            _rigidbody.AddForce(new Vector2(0, 1000));
        }
        private void Update()
        {
            if (_mayCollider != null)
            {
                Jump();
                _mayCollider = null;
            }

            if (math.abs(transform.position.x - aimPoint.x) > 0.1)
            {
                if (aimDirection == MoveDirection.Left && transform.position.x > aimPoint.x)
                    Move(aimDirection);
                if (aimDirection == MoveDirection.Right && transform.position.x < aimPoint.x)
                    Move(aimDirection);
            }
            MoveDirection direction = aimPoint.x > transform.position.x ? MoveDirection.Right : MoveDirection.Left;
            Vector2 hitDirection = new Vector2((float)direction, 0);
            Vector3 sss = new Vector3(hitDirection.x*detact, hitDirection.y);
            Collider2D[] list = Physics2D.OverlapAreaAll(transform.position, transform.position+sss);
            if (list.Length > 0 ) //&& jump && hit.collider.gameObject!=gameObject
            {
                // _mayCollider = hit.collider;
                // Debug.Log(hit.transform.name);
                for (var i = 0; i < list.Length; i++)
                {
                    if (list[i].gameObject != gameObject && jump &&
                        (list[i].transform.position.x - transform.position.x)*(float)direction > 0
                        && (aimPoint.x - list[i].transform.position.x)*(float)direction > 0)
                    {
                        _mayCollider = list[i].GetComponent<BoxCollider2D>();
                        Debug.Log(list[i].gameObject.name);
                    }
                        
                }
            }
        }
        
        private void Move(MoveDirection direction)
        {
            this.transform.position += new Vector3(((float) direction) * moveSpeed * Time.deltaTime, 0, 0);
        }
    }
}