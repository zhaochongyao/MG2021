using System;
using System.Collections;
using System.Threading;
using Constdef;
using DG.Tweening;
using Managers.MapManager;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities.DataStructures;


public class NewPlayer : MonoBehaviour
{
    // attribute
    public float moveSpeed = 3f;            // 水平移动速度
    private Vector3 _resultPoint;
    private Vector3 _playerPoint;

    private Animator _animator;
    // attribute
    public int IsTurnAround =-1;
    public int IsBack = -1;
    public int IsFront = -1;
    public int IsLeft = -1;
    public int IsRight = -1;

    public bool Left = true;

    [SerializeField] private AudioClip _walkSound;
    private AudioSource _audioSource;

    // private SpriteRenderer _renderer;
    private void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;
        _audioSource.clip = _walkSound;
        _audioSource.Play();
        _audioSource.Pause();
        
        // EventCenter.GetInstance().AddEventListener<StarA.Point>(Common.PlayerMoveEvent, MoveTo);
        _animator = GetComponentInChildren<Animator>();
        // _renderer = GetComponentInChildren<SpriteRenderer>();
        
        _animator.speed = 0;
        IsTurnAround = Animator.StringToHash ("IsTurnAround");
        IsBack = Animator.StringToHash ("IsBack");
        IsFront = Animator.StringToHash("IsFront");
        IsLeft = Animator.StringToHash ("IsLeft");
        IsRight = Animator.StringToHash("IsRight");

        _moving = false;
    }

    // private Vector3 ConvertScreenToWorldPoint(Vector3 screenPoint)
    // {
    //     if (Camera.main == null)
    //     {
    //         utils.LogUtil.LogError(Constdef.ConstDefine.CameraNullError);
    //         return transform.position;  // 出错返回自身坐标 即不移动
    //     }
    //     var transformScreen = Camera.main.WorldToScreenPoint(transform.position);
    //     screenPoint.z = transformScreen.z;
    //     var resultPoint = Camera.main.ScreenToWorldPoint(screenPoint);
    //     return resultPoint;
    // }
        
    // private void MoveTo(StarA.Point point)
    // {
    //     if (transform == null || Camera.main == null)
    //     {
    //         return;
    //     }
    //
    //     if (_playerPoint == null)
    //     {
    //         Debug.LogWarning("playerPoint is null");
    //         return;
    //     }
    //     MapManager.GetInstance().ReInit();
    //     MapManager.GetInstance().FindResult(_playerPoint, point);
    //     _resultPoint = point;
    // }

    private bool _moving;
    private Vector3 _target;
    
    public void MoveTowards(Vector3 pos, Action action = null)
    {
        if (_moving)
        {
            return;
        }

        _target = pos;
        Vector3 p1 = transform.position;
        p1.y = pos.y = 0f;    
        float time = (p1 - pos).magnitude / moveSpeed;
        transform.DOMoveX(pos.x, time);
        transform.DOMoveZ(pos.z, time);
        _moving = true;
        
        _audioSource.UnPause();
        
        Wait.Delayed(() =>
        {
            _moving = false;
            if (action != null)
            {
                action.Invoke();
            }
            _audioSource.Pause();
        }, time);
    }
    
    private void Update()
    {
        // if (_playerPoint == null)
        // {
        //     _playerPoint = MapManager.GetInstance().GetPoint(0,  0);
        // }
        // if (_resultPoint != null && _playerPoint != _resultPoint && _playerPoint.child != null)
        if (_moving)
        {
            _animator.SetBool(IsLeft, true);
            // var step = moveSpeed * Time.deltaTime; 
            // var pointPosition = new Vector3(_playerPoint.child.Layout.x, transform.position.y, _playerPoint.child.Layout.z);
            var pointPosition = _target;
            _playerPoint = transform.position;
            _resultPoint = _target;
            // Debug.Log("move to " + pointPosition + "   index = "+_playerPoint.child.x+_playerPoint.child.y);
            bool left = pointPosition.x < transform.localPosition.x;
            if (_playerPoint.x > _resultPoint.x)
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
                _animator.SetBool(IsLeft, true);
                _animator.SetBool(IsLeft, false);
                _animator.SetBool(IsRight, false);
            } else if (_playerPoint.x < _resultPoint.x)
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
                _animator.SetBool(IsLeft, false);
                _animator.SetBool(IsLeft, false);
                _animator.SetBool(IsRight, false);
            } else if (_playerPoint.y > _resultPoint.y)
            {
                if (!_animator.GetBool(IsRight))
                {
                    // transform.DOFlip();
                    _animator.SetBool(IsTurnAround, true);
                }
                else
                {
                    _animator.SetBool(IsTurnAround, false);
                }
                _animator.SetBool(IsBack, false);
                _animator.SetBool(IsLeft, false);
                _animator.SetBool(IsLeft, false);
                _animator.SetBool(IsRight, true);
            } else if (_playerPoint.y <= _resultPoint.y)
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
                _animator.SetBool(IsLeft, false);
                _animator.SetBool(IsLeft, true);
                _animator.SetBool(IsRight, false);
                Debug.Log("向左走");
            } else if (_playerPoint.y > _resultPoint.y)
            {
                if (!_animator.GetBool(IsRight))
                {
                    // transform.DOFlip();
                    _animator.SetBool(IsTurnAround, true);
                }
                else
                {
                    _animator.SetBool(IsTurnAround, false);
                }
                _animator.SetBool(IsBack, false);
                _animator.SetBool(IsLeft, false);
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
                _animator.SetBool(IsLeft, false);
                _animator.SetBool(IsLeft, true);
                _animator.SetBool(IsRight, false);
            }
            // gameObject.transform.localPosition = Vector3.MoveTowards(gameObject.transform.localPosition, pointPosition, step);
            _animator.speed = 1;
            // if (transform.position == pointPosition)
            // {
            //     _playerPoint = _playerPoint.child;
            // }
        }
        else
        {
            _animator.speed = 0;
            _animator.SetBool(IsTurnAround, false);

            // _animator.SetBool(IsMove, false);
        }
    }

}