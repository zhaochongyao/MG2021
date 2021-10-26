using UnityEngine;
using UnityEngine.EventSystems;

namespace StoryLine
{
    public class MouseClicker : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private Transform _target;
        
        private void OnMouseOver()
        {
            Debug.Log("Down");
            if (EventSystem.current.IsPointerOverGameObject() == false && Input.GetMouseButtonDown(0))
            {
                Debug.Log("Click");
                _player.MoveTowards(_target.position);
            }
        }
    }
}