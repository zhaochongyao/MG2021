using Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.DesignPatterns;

namespace Iphone
{
    [System.Serializable]
    public class ThumbnailPictureItem
    {
        [SerializeField] private Sprite _thumbnail;
        [SerializeField] private Sprite _picture;
        [SerializeField] private string _date;

        public Sprite Thumbnail => _thumbnail;
        public Sprite Picture => _picture;
        public string Date => _date;
    }

    public class Album : LSingleton<Album>
    {
        [SerializeField] private GameObject _thumbnailBackground;
        [SerializeField] private Transform _thumbnailLayoutGroup;
        [SerializeField] private GameObject _thumbnailPrefab;

        [SerializeField] private GameObject _pictureObject;
        [SerializeField] private TextMeshProUGUI _pictureDateText;

        [SerializeField] private Image _picture;
        
        private void Start()
        {
            foreach (ThumbnailPictureItem item in GameConfigProxy.Instance.IphoneConfigSO.ThumbnailPictureItems)
            {
                GameObject go = Instantiate(_thumbnailPrefab, _thumbnailLayoutGroup);
                Button button = go.GetComponentInChildren<Button>();
                button.image.sprite = item.Thumbnail;
                button.onClick.AddListener(() =>
                {
                    _thumbnailBackground.SetActive(false);
                    _pictureObject.SetActive(true);
                    _pictureDateText.text = item.Date;
                    _picture.sprite = item.Picture;
                });
            }
        }

        public void BackToThumbnail()
        {
            _thumbnailBackground.SetActive(true);
            _pictureObject.SetActive(false);
        }
    }
}