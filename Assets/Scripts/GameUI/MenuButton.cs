using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameUI
{
    public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Image _normal;
        private Image _selected;
        
        private void Start()
        {
            _normal = GetComponent<Image>();
            _selected = transform.GetChild(0).GetComponent<Image>();
            _normal.color = Color.white;
            _selected.color = Color.clear;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _normal.color = Color.clear;
            _selected.color = Color.white;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _normal.color = Color.white;
            _selected.color = Color.clear;
        }
    }
}