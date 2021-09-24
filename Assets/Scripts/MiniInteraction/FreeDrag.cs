using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MiniInteraction
{
    public class FreeDrag : MonoBehaviour, IDragHandler
    {
        private RectTransform _dragRectTrans;

        private Camera _camera;

        private void Awake()
        {
            _dragRectTrans = GetComponent<RectTransform>();
            _camera = GetComponent<Image>().canvas.worldCamera;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 originPosition = _dragRectTrans.position;
            Vector2 position = _camera.WorldToScreenPoint(originPosition);
            float preserveZ = originPosition.z;
            position += eventData.delta;
            Vector3 newPosition = _camera.ScreenToWorldPoint(position);
            newPosition.z = preserveZ;
            _dragRectTrans.position = newPosition;
        }
    }
}
