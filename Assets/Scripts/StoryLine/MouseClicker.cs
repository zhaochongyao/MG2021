using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StoryLine
{
    public class MouseClicker : MonoBehaviour
    {
        public static event Action<string> MouseClickEvent = delegate { };

        [SerializeField] private string _eventName;
        
        private void OnMouseOver()
        {
            if (EventSystem.current.IsPointerOverGameObject() == false && Input.GetMouseButtonDown(0))
            {
                MouseClickEvent.Invoke(_eventName);
            }
        }
    }
}