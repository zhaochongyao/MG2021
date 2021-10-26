using System.Collections;
using System.Collections.Generic;
using Managers.MapManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseTester : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Player _player;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                bool isCollider = Physics.Raycast(ray, out hit, 1000);
                if (isCollider)
                {
                    Debug.Log(hit.collider.gameObject);
                }
            }
            else
            {
                Debug.Log("UI");
            }
        }
    }
}
