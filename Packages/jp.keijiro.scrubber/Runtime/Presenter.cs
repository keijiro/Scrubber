using UnityEngine;
using UnityEngine.UI;

namespace Scrubber
{
    public sealed class Presenter : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] Deck[] _decks = null;
        [SerializeField] VideoHandler _videoPrefab = null;
        [SerializeField] Text _textUI = null;
        [SerializeField] RawImage _imageUI = null;

        #endregion

        #region Internal objects

        VideoHandler _video;
        (int deck, int page) _position;

        #endregion

        #region Private property and method

        Deck CurrentDeck { get { return _decks[_position.deck]; } }

        void MovePosition(int delta)
        {
            if (delta < 0 && _position.page == 0)
            {
                if (_position.deck > 0)
                {
                    // Go back to the previous deck.
                    _position.deck--;
                    _position.page = CurrentDeck.pageCount - 1;
                }
            }
            else if (delta > 0 && _position.page == CurrentDeck.pageCount - 1)
            {
                if (_position.deck < _decks.Length - 1)
                {
                    // Go to the next deck.
                    _position.deck++;
                    _position.page = 0;
                }
            }
            else
            {
                _position.page += delta;
            }
        }

        static KeyCode GetHotKeyCode(int index)
        {
            if (index < 9) return KeyCode.Alpha1 + index;
            return KeyCode.A + (index - 9);
        }

        #endregion

        #region MonoBehaviour implementation

        void Start()
        {
            UpdatePage();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                // Previous page
                MovePosition(-1);
                UpdatePage();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                // Next page
                MovePosition(+1);
                UpdatePage();
            }
            else
            {
                // Check the deck selection hot keys.
                for (var i = 0; i < _decks.Length; i++)
                {
                    if (Input.GetKeyDown(GetHotKeyCode(i)))
                    {
                        _position = (i, 0);
                        UpdatePage();
                        break;
                    }
                }
            }
        }

        void UpdatePage()
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
            var page = CurrentDeck.GetPage(_position.page);

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

            // Hide the mouse cursor.
            Cursor.visible = false;
        }

        #endregion
    }
}
