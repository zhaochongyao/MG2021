using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MiniInteraction
{
    public class StraightDrag : MonoBehaviour, IDragHandler
    {
        private RectTransform _dragRectTrans;

        private enum DragDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        [SerializeField] private DragDirection _dragDirection;
        
        private Camera _camera;

        private void Awake()
        {
            _dragRectTrans = GetComponent<RectTransform>();
            _camera = GetComponent<Image>().canvas.worldCamera;
            
            /*

            Vector3 origin = _dragRectTrans.position;
            _targetPos = _dragDirection switch
            {
                DragDirection.Up => origin + new Vector3(0f, _dragAmount, 0f),
                DragDirection.Down => origin + new Vector3(0f, -_dragAmount, 0f),
                DragDirection.Left => origin + new Vector3(-_dragAmount, 0f, 0f),
                DragDirection.Right => origin + new Vector3(_dragAmount, 0f, 0f),
                _ => _targetPos
            };
            
            */
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 originPosition = _dragRectTrans.position;
            Vector2 position = _camera.WorldToScreenPoint(originPosition);
            float preserveZ = originPosition.z;

            if (_dragDirection == DragDirection.Down || _dragDirection == DragDirection.Up)
            {
                position.y += eventData.delta.y;
            }
            else if (_dragDirection == DragDirection.Left || _dragDirection == DragDirection.Right)
            {
                position.x += eventData.delta.x;
            }

            Vector3 newPosition = _camera.ScreenToWorldPoint(position);
            newPosition.z = preserveZ;
            _dragRectTrans.position = newPosition;
        }
    }
}