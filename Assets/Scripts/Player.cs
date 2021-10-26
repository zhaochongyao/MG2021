using System;
using System.Collections;
using System.Threading;
using Constdef;
using DG.Tweening;
using Managers.MapManager;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;


public class Player : MonoBehaviour
{
    // attribute
    public float moveSpeed = 3f;            // 水平移动速度
    private StarA.Point _resultPoint;
    private StarA.Point _playerPoint;

    private Animator _animator;
    // attribute
    public int IsTurnAround =-1;
    public int IsBack = -1;
    public int IsFront = -1;
    public int IsLeft = -1;
    public int IsRight = -1;
    public bool Left = true;
    private void Start()
    {
        EventCenter.GetInstance().AddEventListener<StarA.Point>(Common.PlayerMoveEvent, MoveTo);
        _animator = GetComponentInChildren<Animator>();
        _animator.speed = 0;
        IsTurnAround = Animator.StringToHash ("IsTurnAround");
        IsBack = Animator.StringToHash ("IsBack");
        IsFront = Animator.StringToHash ("IsFront");
        IsLeft = Animator.StringToHash("IsLeft");
        IsRight = Animator.StringToHash("IsRight");
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

        if (_playerPoint == null)
        {
            Debug.LogWarning("playerPoint is null");
            return;
        }
        MapManager.GetInstance().ReInit();
        MapManager.GetInstance().FindResult(_playerPoint, point);
        _resultPoint = point;
    }
    
    private void Update()
    {
        if (_playerPoint == null)
        {
            _playerPoint = MapManager.GetInstance().GetPoint(0,  0);
        }
        if (_resultPoint != null && _playerPoint != _resultPoint && _playerPoint.child != null)
        {
            _animator.SetBool(IsFront, true);
            var step = moveSpeed * Time.deltaTime; 
            var pointPosition = new Vector3(_playerPoint.child.Layout.x, transform.position.y, _playerPoint.child.Layout.z);
            // Debug.Log("move to " + pointPosition + "   index = "+_playerPoint.child.x+_playerPoint.child.y);
            bool left = _resultPoint.Layout.x < transform.localPosition.x;
            
            // Debug.Log(Math.Abs(pointPosition.z - transform.localPosition.z));
            if (_playerPoint.x > _resultPoint.x && _playerPoint.y == _resultPoint.y)
            {
                if (!_animator.GetBool(IsFront))
                {
                    _animator.SetBool(IsTurnAround, true);
                }
                else
                {
                    _animator.SetBool(IsTurnAround, false);
                }
                _animator.SetBool(IsBack, false);
                _animator.SetBool(IsFront, true);
                _animator.SetBool(IsLeft, false);
                _animator.SetBool(IsRight, false);
            } else if (_playerPoint.x < _resultPoint.x && _playerPoint.y == _resultPoint.y)
            {
                if (!_animator.GetBool(IsBack))
                {
                    _animator.SetBool(IsTurnAround, true);
                }
                else
                {
                    _animator.SetBool(IsTurnAround, false);
                }
                _animator.SetBool(IsBack, true);
                _animator.SetBool(IsFront, false);
                _animator.SetBool(IsLeft, false);
                _animator.SetBool(IsRight, false);
            } else if (_playerPoint.y > _resultPoint.y && _playerPoint.x == _resultPoint.x)
            {
                if (!_animator.GetBool(IsRight))
                {
                    transform.DOFlip();
                    _animator.SetBool(IsTurnAround, true);
                }
                else
                {
                    _animator.SetBool(IsTurnAround, false);
                }
                _animator.SetBool(IsBack, false);
                _animator.SetBool(IsFront, false);
                _animator.SetBool(IsLeft, false);
                _animator.SetBool(IsRight, true);
            } else if (_playerPoint.y < _resultPoint.y && _playerPoint.x == _resultPoint.x)
            {
                if (!_animator.GetBool(IsLeft))
                {
                    _animator.SetBool(IsTurnAround, true);
                }
                else
                {
                    _animator.SetBool(IsTurnAround, false);
                }
                _animator.SetBool(IsBack, false);
                _animator.SetBool(IsFront, false);
                _animator.SetBool(IsLeft, true);
                _animator.SetBool(IsRight, false);
                Debug.Log("向左走");
            } else if (_playerPoint.y > _resultPoint.y)
            {
                if (!_animator.GetBool(IsRight))
                {
                    transform.DOFlip();
                    _animator.SetBool(IsTurnAround, true);
                }
                else
                {
                    _animator.SetBool(IsTurnAround, false);
                }
                _animator.SetBool(IsBack, false);
                _animator.SetBool(IsFront, false);
                _animator.SetBool(IsLeft, false);
                _animator.SetBool(IsRight, true);
            } else if (_playerPoint.y < _resultPoint.y)
            {
                if (!_animator.GetBool(IsLeft))
                {
                    _animator.SetBool(IsTurnAround, true);
                }
                else
                {
                    _animator.SetBool(IsTurnAround, false);
                }
                _animator.SetBool(IsBack, false);
                _animator.SetBool(IsFront, false);
                _animator.SetBool(IsLeft, true);
                _animator.SetBool(IsRight, false);
            }
            // if (_resultPoint.Layout.z > transform.localPosition.z && Math.Abs(_resultPoint.Layout.z - transform.localPosition.z) > 0.1f)
            // {
            //     if (!_animator.GetBool(IsBack))
            //     {
            //         _animator.SetBool(IsTurnAround, true);
            //     }
            //     else
            //     {
            //         _animator.SetBool(IsTurnAround, false);
            //     }
            //     _animator.SetBool(IsBack, true);
            //     _animator.SetBool(IsFront, false);
            //     _animator.SetBool(IsLeft, false);
            //     _animator.SetBool(IsRight, false);
            //     Debug.Log("向上走");
            //     if (math.abs(_resultPoint.Layout.x - transform.localPosition.x) >
            //         math.abs(_resultPoint.Layout.z - transform.localPosition.z))
            //     {
            //         if (left)
            //         {
            //             if (!_animator.GetBool(IsLeft))
            //             {
            //                 _animator.SetBool(IsTurnAround, true);
            //             }
            //             else
            //             {
            //                 _animator.SetBool(IsTurnAround, false);
            //             }
            //             _animator.SetBool(IsBack, false);
            //             _animator.SetBool(IsFront, false);
            //             _animator.SetBool(IsLeft, true);
            //             _animator.SetBool(IsRight, false);
            //             Debug.Log("向左走");
            //         }
            //         else
            //         {
            //             if (!_animator.GetBool(IsRight))
            //             {
            //                 transform.DOFlip();
            //                 _animator.SetBool(IsTurnAround, true);
            //             }
            //             else
            //             {
            //                 _animator.SetBool(IsTurnAround, false);
            //             }
            //             _animator.SetBool(IsBack, false);
            //             _animator.SetBool(IsFront, false);
            //             _animator.SetBool(IsLeft, false);
            //             _animator.SetBool(IsRight, true);
            //             Debug.Log("向右走");
            //         }
            //     }
            // }
            // else if (_resultPoint.Layout.z < transform.localPosition.z && Math.Abs(_resultPoint.Layout.z - transform.localPosition.z) > 0.1f)
            // {
            //     if (!_animator.GetBool(IsFront))
            //     {
            //         _animator.SetBool(IsTurnAround, true);
            //     }
            //     else
            //     {
            //         _animator.SetBool(IsTurnAround, false);
            //     }
            //     _animator.SetBool(IsBack, false);
            //     _animator.SetBool(IsFront, true);
            //     _animator.SetBool(IsLeft, false);
            //     _animator.SetBool(IsRight, false);
            //     Debug.Log("向下走");
            //     if (math.abs(_resultPoint.Layout.x - transform.localPosition.x) >
            //         math.abs(_resultPoint.Layout.z - transform.localPosition.z))
            //     {
            //         if (left)
            //         {
            //             if (!_animator.GetBool(IsLeft))
            //             {
            //                 _animator.SetBool(IsTurnAround, true);
            //             }
            //             else
            //             {
            //                 _animator.SetBool(IsTurnAround, false);
            //             }
            //             _animator.SetBool(IsBack, false);
            //             _animator.SetBool(IsFront, false);
            //             _animator.SetBool(IsLeft, true);
            //             _animator.SetBool(IsRight, false);
            //             Debug.Log("向左走");
            //         }
            //         else
            //         {
            //             if (!_animator.GetBool(IsRight))
            //             {
            //                 transform.DOFlip();
            //                 _animator.SetBool(IsTurnAround, true);
            //             }
            //             else
            //             {
            //                 _animator.SetBool(IsTurnAround, false);
            //             }
            //             _animator.SetBool(IsBack, false);
            //             _animator.SetBool(IsFront, false);
            //             _animator.SetBool(IsLeft, false);
            //             _animator.SetBool(IsRight, true);
            //             Debug.Log("向右走");
            //         }
            //     }
            // }

            gameObject.transform.localPosition = Vector3.MoveTowards(gameObject.transform.localPosition, pointPosition, step);
            _animator.speed = 1;
            if (transform.position == pointPosition)
            {
                _playerPoint = _playerPoint.child;
            }
        }
        else
        {
            _animator.speed = 0;
            _animator.SetBool(IsTurnAround, false);
            // _animator.SetBool(IsMove, false);
        }
    }

}