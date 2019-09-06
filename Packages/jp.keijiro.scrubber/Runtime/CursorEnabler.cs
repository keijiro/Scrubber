using UnityEngine;
using UnityEngine.UI;

namespace Scrubber
{
    public sealed class CursorEnabler : MonoBehaviour
    {
        [SerializeField] float _threshold = 0.025f;

        Vector3 _prevPosition;

        void Start()
        {
            _prevPosition = Input.mousePosition;
        }

        void Update()
        {
            var diff = (Input.mousePosition - _prevPosition).magnitude;
            if (diff > _threshold * Screen.height) Cursor.visible = true;
            _prevPosition = Input.mousePosition;
        }
    }
}
