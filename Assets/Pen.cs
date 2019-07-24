using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scrubber
{
    public sealed class Pen : MonoBehaviour
    {
        [SerializeField, HideInInspector] Shader _shader = null;
        [SerializeField] RenderTexture _renderTexture = null;
        [SerializeField] Color _color = Color.white;

        Material _material;

        public void OnDrag(BaseEventData baseData)
        {
            var data = (PointerEventData)baseData;
            data.Use();

            var area = data.pointerDrag.GetComponent<RectTransform>();
            var p0 = area.InverseTransformPoint(data.position - data.delta);
            var p1 = area.InverseTransformPoint(data.position);

            var scale = new Vector3(2 / area.rect.width, -2 / area.rect.height, 0);
            p0 = Vector3.Scale(p0, scale);
            p1 = Vector3.Scale(p1, scale);

            var eraser = (data.button == PointerEventData.InputButton.Right);
            eraser |= Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            var prevRT = RenderTexture.active;
            RenderTexture.active = _renderTexture;

            _material.SetVector("_Point0", p0);
            _material.SetVector("_Point1", p1);
            _material.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Lines, 2, 1);

            RenderTexture.active = prevRT;
        }

        void Start()
        {
            _material = new Material(_shader);
            _material.color = _color;
        }

        void OnDestroy()
        {
            Destroy(_material);
        }
    }
}
