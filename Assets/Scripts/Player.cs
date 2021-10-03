using System;
using System.Collections;
using System.Threading;
using Managers.MapManager;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;


public class Player : MonoBehaviour
{
    // component
    private Rigidbody2D _rigidbody;    
    // component
        
    // other object
    private Collider2D _mayCollider;
    // other object
        
    // attribute
    public bool jump = false;               // 
    public Vector3 aimPoint;                // 鼠标点击坐标(世界坐标)
    public float moveSpeed = 3f;            // 水平移动速度
    public float detact = 3f;               // 障碍物检测距离
    private Constdef.MoveDirection _aimDirection;     // 移动目标方向
    // attribute

    private StarA.Point ResultPoint;
    private StarA.Point PlayerPoint;
    private void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        aimPoint = transform.position;
        EventCenter.GetInstance().AddEventListener<StarA.Point>("player_move_event", MoveTo);
    }

    private Vector3 ConvertScreenToWorldPoint(Vector3 screenPoint)
    {
        if (Camera.main == null)
        {
            utils.LogUtil.LogError(Constdef.ConstDefine.CameraNullError);
            return transform.position;  // 出错返回自身坐标 即不移动
        }
        var transformScreen = Camera.main.WorldToScreenPoint(transform.position);
        screenPoint.z = transformScreen.z;
        var resultPoint = Camera.main.ScreenToWorldPoint(screenPoint);
        return resultPoint;
    }
        
    private void MoveTo(StarA.Point point)
    {
        if (transform == null || Camera.main == null)
        {
            return;
        }

        if (PlayerPoint == null)
        {
            Debug.LogError("playerPoint is null");
            return;
        }
        MapManager.GetInstance().ReInit();
        MapManager.GetInstance().FindResult(PlayerPoint, point);
        ResultPoint = point;
        // jump = true;
        // aimPoint = ConvertScreenToWorldPoint(point);
        // if (transform.position.x > aimPoint.x)
        // {
        //     _aimDirection = MoveDirection.Left;
        // }
        //     
        // if (transform.position.x < aimPoint.x)
        // {
        //     _aimDirection = MoveDirection.Right;
        // }
    }

    private void Jump()
    {
        jump = false;
        _rigidbody.AddForce(new Vector2(0, 1000));
    }
    private void Update()
    {
        if (PlayerPoint == null)
        {
            PlayerPoint = MapManager.GetInstance().GetPoint(0,  0);
        }
        if (ResultPoint != null && PlayerPoint != ResultPoint && PlayerPoint.child != null)
        {
            var step = moveSpeed * Time.deltaTime; 
            var pointPosition = new Vector3(PlayerPoint.child.Layout.x, transform.position.y, PlayerPoint.child.Layout.z);
            Debug.Log("move to " + pointPosition + "   index = "+PlayerPoint.child.x+PlayerPoint.child.y);
            gameObject.transform.localPosition = Vector3.MoveTowards(gameObject.transform.localPosition, pointPosition, step); 
            if (transform.position == pointPosition)
            {
                PlayerPoint = PlayerPoint.child;
            }
        }
        // if (_mayCollider != null)
        // {
        //     Jump();
        //     _mayCollider = null;
        // }

        // if (math.abs(transform.position.x - aimPoint.x) > 0.1)
        // {
        //     if (_aimDirection == MoveDirection.Left && transform.position.x > aimPoint.x)
        //         Move(_aimDirection);
        //     if (_aimDirection == MoveDirection.Right && transform.position.x < aimPoint.x)
        //         Move(_aimDirection);
        // }

        // var transformPosition = transform.position;
        // var direction = aimPoint.x > transformPosition.x ? MoveDirection.Right : MoveDirection.Left;
        // var hitDirection = new Vector2((float)direction, 0);
        // var sss = new Vector3(hitDirection.x*detact, hitDirection.y);
        // var list = Physics2D.OverlapAreaAll(transformPosition, transformPosition+sss);
        // if (list.Length > 0)
        // {
        //     foreach (var itemCollider in list)
        //     {
        //         if (itemCollider.gameObject != gameObject && jump && utils.CommonUtil.CheckPointInColliderArea(aimPoint, itemCollider))
        //         {
        //             _mayCollider = itemCollider.GetComponent<BoxCollider2D>();
        //             Debug.Log(itemCollider.gameObject.name);
        //         }
        //     }
        // }
    }

    private void Move(Constdef.MoveDirection direction)
    {

        this.transform.position += Constdef.Direction.GetDirectionVector(direction) * moveSpeed * Time.deltaTime;

    }

}