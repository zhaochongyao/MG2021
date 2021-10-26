using UnityEngine;
using UnityEngine.EventSystems;

namespace StoryLine
{
    public class MouseMoveClicker : MonoBehaviour
    {
        [SerializeField] private NewPlayer _newPlayer;
        [SerializeField] private Transform _target;
        
        private void OnMouseOver()
        {
            Debug.Log("Down");
            if (EventSystem.current.IsPointerOverGameObject() == false && Input.GetMouseButtonDown(0))
            {
                Debug.Log("Click");
                _newPlayer.MoveTowards(_target.position);
            }
        }
    }
}