using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StoryLine
{
    public class OpenDoor : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        [SerializeField] private Collider _collider;
        
        public static event Action DoorOpen = delegate { };
        
        private void OnMouseOver()
        {
            if (EventSystem.current.IsPointerOverGameObject() == false && Input.GetMouseButtonDown(0))
            {
                _animator.enabled = true;
                _collider.enabled = false;
                DoorOpen.Invoke();
            }
        }
    }
}