using UnityEngine;
using UnityEngine.UI;

namespace Scrubber
{
    [System.Serializable]
    public struct Page
    {
        public string videoName;
        public bool autoPlay;
        public bool loop;
        public string text;
        public Texture image;
    }

    public sealed class Presentation : MonoBehaviour
    {
        [SerializeField, HideInInspector] VideoHandler _videoPrefab = null;
        [SerializeField] Text _textUI = null;
        [SerializeField] RawImage _imageUI = null;
        [SerializeField] Page[] _pages = null;

        VideoHandler _video;
        int _index;

        Vector3 _mousePosition;

        void Start()
        {
            UpdatePage(_index);
            Cursor.visible = false;
            _mousePosition = Input.mousePosition;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) && _index > 0)
                UpdatePage(--_index);

            if (Input.GetKeyDown(KeyCode.RightArrow) && _index < _pages.Length - 1)
                UpdatePage(++_index);

            if ((Input.mousePosition - _mousePosition).magnitude > 40)
                Cursor.visible = true;

            _mousePosition = Input.mousePosition;
        }

        void UpdatePage(int index)
        {
            var page = _pages[index];

            if (_video != null)
            {
                Destroy(_video.gameObject);
                _video = null;
            }

            if (!string.IsNullOrEmpty(page.videoName))
            {
                _video = Instantiate(_videoPrefab);
                _video.Open(page.videoName, page.autoPlay, page.loop);
            }

            if (page.image != null)
            {
                _imageUI.texture = page.image;
                _imageUI.enabled = true;
            }
            else
            {
                _imageUI.texture = null;
                _imageUI.enabled = false;
            }

            _textUI.text = page.text.Replace("<br>", "\n");

            FindObjectOfType<Pen>().Clear();

            Cursor.visible = false;
        }
    }
}
