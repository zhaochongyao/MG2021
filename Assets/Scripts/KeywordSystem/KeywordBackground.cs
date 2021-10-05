using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace KeywordSystem
{
    public class KeywordBackground : MonoBehaviour
    {
        private Image _background;
        private TextMeshProUGUI _text;

        private bool _reset;
        
        private void Awake()
        {
            _background = GetComponent<Image>();
        }

        private void OnEnable()
        {
            _reset = true;
        }

        private void Update()
        {
            if (_reset)
            {
                _text = transform.parent.GetComponentInChildren<TextMeshProUGUI>();
                _reset = false;
            }
            
            _background.color = new Color
            {
                r = _background.color.r,
                g = _background.color.g,
                b = _background.color.b,
                a = _text.color.a
            };
            
            if (_text.color.a == 0f)
            {
                ObjectPool.Return(gameObject);
            }
        }
    }
}