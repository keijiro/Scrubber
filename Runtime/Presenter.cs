using UnityEngine;
using UnityEngine.UI;

namespace Scrubber
{
    public sealed class Presenter : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] Deck _deck = null;
        [SerializeField] VideoHandler _videoPrefab = null;
        [SerializeField] Text _textUI = null;
        [SerializeField] RawImage _imageUI = null;

        #endregion

        #region Internal objects

        VideoHandler _video;
        int _index;
        Vector3 _mousePosition;

        #endregion

        #region MonoBehaviour implementation

        void Start()
        {
            UpdatePage(_index);
            Cursor.visible = false;
            _mousePosition = Input.mousePosition;
        }

        void Update()
        {
            // Previous page
            if (Input.GetKeyDown(KeyCode.LeftArrow) && _index > 0)
                UpdatePage(--_index);

            // Next page
            var lastPage = _deck.pageCount - 1;
            if (Input.GetKeyDown(KeyCode.RightArrow) && _index < lastPage)
                UpdatePage(++_index);

            // Enable the mouse cursor when it's moved.
            if ((Input.mousePosition - _mousePosition).magnitude > 40)
                Cursor.visible = true;

            _mousePosition = Input.mousePosition;
        }

        void UpdatePage(int index)
        {
            // Destroy the previous video player.
            if (_video != null)
            {
                Destroy(_video.gameObject);
                _video = null;
            }

            // Clear pen drawing on the previous page.
            FindObjectOfType<Pen>().Clear();

            // Current page
            var page = _deck.GetPage(index);

            // Video element: Instantiate a video player if it exists.
            if (!string.IsNullOrEmpty(page.videoName))
            {
                _video = Instantiate(_videoPrefab);
                _video.Open(page.videoName, page.autoPlay, page.loop);
            }

            // Image element
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

            // Text element
            _textUI.text = page.text.Replace("<br>", "\n");

            // Hide the mouse cursor again.
            Cursor.visible = false;
        }

        #endregion
    }
}
