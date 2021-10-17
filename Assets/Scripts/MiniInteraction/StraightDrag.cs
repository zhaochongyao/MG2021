using UnityEngine;
using UnityEngine.EventSystems;

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

        [SerializeField] private bool _canReverse;
        
        private void Awake()
        {
            _dragRectTrans = GetComponent<RectTransform>();
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            Vector3 originPosition = _dragRectTrans.position;
            Camera eventCamera = eventData.pressEventCamera;
            Vector2 position = eventCamera.WorldToScreenPoint(originPosition);
            float preserveZ = originPosition.z;

            if (_dragDirection == DragDirection.Down || _dragDirection == DragDirection.Up)
            {
                position.y += eventData.delta.y;
            }
            else if (_dragDirection == DragDirection.Left || _dragDirection == DragDirection.Right)
            {
                position.x += eventData.delta.x;
            }

            Vector3 newPosition = eventCamera.ScreenToWorldPoint(position);
            newPosition.z = preserveZ;
            _dragRectTrans.position = newPosition;
        }
    }
}