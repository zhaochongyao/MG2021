using System;
using System.Collections;
using System.Threading;
using Constdef;
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
    // attribute


    private void Start()
    {
        EventCenter.GetInstance().AddEventListener<StarA.Point>(Common.PlayerMoveEvent, MoveTo);
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
            var step = moveSpeed * Time.deltaTime; 
            var pointPosition = new Vector3(_playerPoint.child.Layout.x, transform.position.y, _playerPoint.child.Layout.z);
            Debug.Log("move to " + pointPosition + "   index = "+_playerPoint.child.x+_playerPoint.child.y);
            gameObject.transform.localPosition = Vector3.MoveTowards(gameObject.transform.localPosition, pointPosition, step); 
            if (transform.position == pointPosition)
            {
                _playerPoint = _playerPoint.child;
            }
        }
    }

}