using System.Collections;
using System.Text;
using Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Iphone.ChatSystem
{
    public class ChatLineWithName : MonoBehaviour
    {
        [SerializeField] private Image _avatar;
        [SerializeField] private TextMeshProUGUI _chatText;
        [SerializeField] private TextMeshProUGUI _chatterName;
        private RectTransform _rectTransform;
        [SerializeField] private RectTransform _textBackgroundRectTrans;
        
        [SerializeField] private Image _memePic;
        [SerializeField] private RectTransform _picMask;
        
        private int _maxHorizontalChar;
        private StringBuilder _sb;

        private void Awake()
        {
            _maxHorizontalChar = GameConfigProxy.Instance.IphoneConfigSO.MaxHorizontalChar;
            _sb = new StringBuilder();
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Set(ChatLine chatLine)
        {
            StartCoroutine(SetCo(chatLine));
        }

        private IEnumerator SetCo(ChatLine chatLine)
        {
            _avatar.sprite = chatLine.ChatterSO.Avatar;
            _chatterName.text = chatLine.ChatterSO.ChatterName;

            if (chatLine.MemePic == null)
            {
                _textBackgroundRectTrans.gameObject.SetActive(true);

                _sb.Clear();
                for (int i = 0; i < chatLine.ChatText.Length; ++i)
                {
                    _sb.Append(chatLine.ChatText[i]);
                    if (i % _maxHorizontalChar == _maxHorizontalChar - 1 && i != chatLine.ChatText.Length - 1)
                    {
                        _sb.Append('\n');
                    }
                }

                _chatText.text = _sb.ToString();

                yield return null;

                _rectTransform.sizeDelta = new Vector2
                {
                    x = _rectTransform.sizeDelta.x,
                    y = Mathf.Abs(_chatterName.rectTransform.localPosition.y) * 2 + _textBackgroundRectTrans.sizeDelta.y
                };
            }
            else
            {
                _picMask.gameObject.SetActive(true);
                _memePic.sprite = chatLine.MemePic;
                Vector2 size = chatLine.MemePic.bounds.extents;
                float ratio = size.x / size.y;
                if (size.x > size.y)
                {
                    _picMask.sizeDelta = new Vector2
                    {
                        x = GameConfigProxy.Instance.IphoneConfigSO.MaxMemePicWidth,
                        y = GameConfigProxy.Instance.IphoneConfigSO.MaxMemePicWidth / ratio
                    };
                }
                else
                {
                    _picMask.sizeDelta = new Vector2
                    {
                        x = GameConfigProxy.Instance.IphoneConfigSO.MaxMemePicHeight * ratio,
                        y = GameConfigProxy.Instance.IphoneConfigSO.MaxMemePicHeight
                    };
                }

                yield return null;

                _rectTransform.sizeDelta = new Vector2
                {
                    x = _rectTransform.sizeDelta.x,
                    y = Mathf.Abs(_chatterName.rectTransform.localPosition.y) * 2 + _picMask.sizeDelta.y
                };
            }
        }
    }
}