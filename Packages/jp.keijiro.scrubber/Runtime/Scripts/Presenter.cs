using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

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

    #region Position controller

    (int deck, int page) _position;

    Deck CurrentDeck { get { return Decks[_position.deck]; } }

    void MovePosition(int delta)
    {
        if (delta < 0 && _position.page == 0)
        {
            if (_position.deck > 0)
            {
                // Previous deck
                _position.deck--;
                _position.page = CurrentDeck.pageCount - 1;
            }
        }
        else if (delta > 0 && _position.page == CurrentDeck.pageCount - 1)
        {
            if (_position.deck < Decks.Length - 1)
            {
                // Next deck
                _position.deck++;
                _position.page = 0;
            }
        }
        else
        {
            _position.page += delta;
        }
    }

    #endregion

    #region UI controller

    VideoHandler _video;

    void UpdatePage()
    {
        // Current page
        var page = CurrentDeck.GetPage(_position.page);

        // UI document
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Scribble reset
        GetComponent<ScribbleBackend>().ClearCanvas();

        // Mouse cursor reset
        Cursor.visible = false;

        // Existing video player termination
        if (_video != null)
        {
            Destroy(_video.gameObject);
            _video = null;
        }

        // Video player instantiation
        if (!string.IsNullOrEmpty(page.videoName))
        {
            _video = Instantiate(_videoPrefab);
            _video.Open(page.videoName, page.autoPlay, page.loop);
        }

        // Video element
        root.Q<VisualElement>("video").style.backgroundImage =
          _video == null ? null : Background.FromTexture2D(_video.VideoAsTexture);

        // Image element
        root.Q<VisualElement>("image").style.backgroundImage =
          page.image == null ? null : Background.FromTexture2D(page.image as Texture2D);

        // Text element
        root.Q<Label>("text").text = page.text;
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
      => UpdatePage();

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
            // Deck selection hot keys
            for (var i = 0; i < Decks.Length; i++)
            {
                if (keys[Key.Digit1 + i].wasPressedThisFrame)
                {
                    _position = (i, 0);
                    UpdatePage();
                    break;
                }
            }
        }

        if (_video != null)
        {
            // Video player: Wheel parameters
            _video.WheelSpeed = JogSpeed;
            _video.TweenSpeed = JogSense;
        }
    }

    #endregion
}

} // namespace Scrubber
