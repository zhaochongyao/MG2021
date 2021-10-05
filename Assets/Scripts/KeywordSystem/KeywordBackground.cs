using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace KeywordSystem
{
    /// <summary>
    /// 关键词背景方块（暗色字体使用）
    /// </summary>
    public class KeywordBackground : MonoBehaviour
    {
        private Image _background;
        private TextMeshProUGUI _text;

        private void Awake()
        {
            _background = GetComponent<Image>();
        }

        private void OnEnable()
        {
            _background.color = new Color(0f, 0f, 0f, 0f);
            _text = transform.parent.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Update()
        {
            // 同步透明度变化
            _background.color = new Color
            {
                r = _background.color.r,
                g = _background.color.g,
                b = _background.color.b,
                a = _text.color.a
            };
            
            // 透明度为零时，自动回收
            if (_text.color.a == 0f)
            {
                ObjectPool.Return(gameObject);
            }
        }
    }
}