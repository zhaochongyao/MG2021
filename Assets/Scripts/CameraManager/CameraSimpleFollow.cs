using System.Collections;
using System.Collections.Generic;
using Constdef;
using UnityEngine;
using utils;

public class CameraSimpleFollow : MonoBehaviour
{
    public GameObject Character;
    public float moveSpeed = 1.0f;
    public float offset = 0f;
    void Start()
    {
        if (Character == null)
        {
            LogUtil.LogError(new MyError("camera follow character is null", 6001));
        }
    }

    void Update()
    {
        if (Character != null)
        {
            var currentPosition = transform.position;
            currentPosition = Vector3.MoveTowards(currentPosition,
                new Vector3(Character.transform.position.x + offset, currentPosition.y, currentPosition.z),
                moveSpeed * Time.deltaTime);
            transform.position = currentPosition;
        }
    }
}
