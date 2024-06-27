using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Scrubber {

public sealed class Pen : MonoBehaviour
{
    #region Public properties

    [field:SerializeField] public RenderTexture TargetTexture { get; set; }
    [field:SerializeField] public Color StrokeColor { get; set; } = Color.white;
    [field:SerializeField] public float StrokeWidth { get; set; } = 0.01f;

    #endregion

    #region Package asset references

    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Public methods

    public void Clear()
    {
        var prevRT = RenderTexture.active;
        RenderTexture.active = TargetTexture;
        GL.Clear(false, true, Color.clear);
        RenderTexture.active = prevRT;
        Cursor.visible = false;
    }

    #endregion

    #region UI callbacks

    Material _material;

    public void OnDrag(BaseEventData baseData)
    {
        var data = (PointerEventData)baseData;
        if (data.button != PointerEventData.InputButton.Left) return;

        data.Use();

        var area = data.pointerDrag.GetComponent<RectTransform>();
        var p0 = area.InverseTransformPoint(data.position - data.delta);
        var p1 = area.InverseTransformPoint(data.position);

        var scale = new Vector3(2 / area.rect.width, -2 / area.rect.height, 0);
        p0 = Vector3.Scale(p0, scale);
        p1 = Vector3.Scale(p1, scale);

        var eraser = Keyboard.current.shiftKey.isPressed;

        var prevRT = RenderTexture.active;
        RenderTexture.active = TargetTexture;

        _material.SetVector("_Point0", p0);
        _material.SetVector("_Point1", p1);
        _material.SetColor("_Color", eraser ? Color.clear : StrokeColor);
        _material.SetFloat("_Width", (eraser ? 4 : 1) * StrokeWidth);
        _material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Triangles, 12, 1);

        RenderTexture.active = prevRT;
    }

    public void OnPointerClick(BaseEventData baseData)
    {
        var data = (PointerEventData)baseData;
        if (data.button == PointerEventData.InputButton.Right)
        {
            data.Use();
            Clear();
        }
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _material = new Material(_shader);
        Clear();
    }

    void OnDestroy()
      => Destroy(_material);

    #endregion
}

} // namespace Scrubber
