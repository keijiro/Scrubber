using UnityEngine;
using UnityEngine.InputSystem;

namespace Scrubber {

public sealed class CursorEnabler : MonoBehaviour
{
    [field:SerializeField] public float Threshold { get; set; } = 0.025f;

    Vector2 _prevPosition;

    void Start()
      => _prevPosition = Mouse.current.position.value;

    void Update()
    {
        var pos = Mouse.current.position.value;
        var diff = (pos - _prevPosition).magnitude;
        if (diff > Threshold * Screen.height) Cursor.visible = true;
        _prevPosition = pos;
    }
}

} // namespace Scrubber
