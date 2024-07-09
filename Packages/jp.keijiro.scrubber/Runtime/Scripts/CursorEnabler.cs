using UnityEngine;
using UnityEngine.InputSystem;

namespace Scrubber {

public sealed class CursorEnabler : MonoBehaviour
{
    [field:SerializeField] public float Threshold { get; set; } = 0.025f;

    Vector2 _position;

    void Start()
    {
        Cursor.visible = false;
        _position = Mouse.current.position.value;
    }

    void Update()
    {
        var mouse = Mouse.current;
        var keys = Keyboard.current;

        // Cursor activation by mouse motion
        var pos = Mouse.current.position.value;
        var diff = (pos - _position).magnitude;
        if (diff > Threshold * Screen.height) Cursor.visible = true;
        _position = pos;

        // Cusror switching on mouse clicks
        if (mouse.leftButton.wasPressedThisFrame) Cursor.visible = true;
        if (mouse.rightButton.wasPressedThisFrame) Cursor.visible = false;

        // Cursor deactivation on any key presses
        if (keys.anyKey.wasPressedThisFrame) Cursor.visible = false;
    }
}

} // namespace Scrubber
