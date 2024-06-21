using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Scrubber {

public sealed class Presenter : MonoBehaviour
{
    #region Public properties

    [field:SerializeField] public float JogSpeed { get; set; } = 1;
    [field:SerializeField] public float JogSense { get; set; } = 8;
    [field:SerializeField] public Deck[] Decks { get; set; } = null;

    #endregion

    #region Package asset references

    [SerializeField] VideoHandler _videoPrefab = null;

    #endregion

    #region Hierarchy object references

    Text _textUI;
    RawImage _imageUI;

    #endregion

    #region Internal objects

    VideoHandler _video;
    (int deck, int page) _position;

    #endregion

    #region Private property and method

    Deck CurrentDeck { get { return Decks[_position.deck]; } }

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
            if (_position.deck < Decks.Length - 1)
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

    static Key GetKeyFromIndex(int index)
    {
        if (index < 9) return Key.Digit1 + index;
        return Key.A + (index - 9);
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _textUI = GetComponentInChildren<Text>();
        _imageUI = GetComponentInChildren<RawImage>();
        UpdatePage();
    }

    void Update()
    {
        var keys = Keyboard.current;
        if (keys.leftArrowKey.wasPressedThisFrame)
        {
            // Previous page
            MovePosition(-1);
            UpdatePage();
        }
        else if (keys.rightArrowKey.wasPressedThisFrame)
        {
            // Next page
            MovePosition(+1);
            UpdatePage();
        }
        else
        {
            // Check the deck selection hot keys.
            for (var i = 0; i < Decks.Length; i++)
            {
                if (keys[GetKeyFromIndex(i)].wasPressedThisFrame)
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
        FindFirstObjectByType<Pen>().Clear();

        // Current page
        var page = CurrentDeck.GetPage(_position.page);

        // Video element: Instantiate a video player if it exists.
        if (!string.IsNullOrEmpty(page.videoName))
        {
            _video = Instantiate(_videoPrefab);
            _video.WheelSpeed = JogSpeed;
            _video.TweenSpeed = JogSense;
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

} // namespace Scrubber
