using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Scrubber
{
    public sealed class CursorEnabler : MonoBehaviour
    {
        [SerializeField] float _threshold = 0.025f;

        Vector2 _prevPosition;

        void Start()
        {
            _prevPosition = Mouse.current.position.value;
        }

        void Update()
        {
            var pos = Mouse.current.position.value;
            var diff = (pos - _prevPosition).magnitude;
            if (diff > _threshold * Screen.height) Cursor.visible = true;
            _prevPosition = pos;
        }
    }
}
