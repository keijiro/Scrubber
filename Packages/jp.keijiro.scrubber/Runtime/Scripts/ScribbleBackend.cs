using UnityEngine;
using UnityEngine.UIElements;

namespace Scrubber {

public class ScribbleBackend : MonoBehaviour
{
    #region Public properties

    [field:SerializeField] public string ElementID { get; set; } = "scribble";
    [field:SerializeField] public int Resolution { get; set; } = 1024;
    [field:SerializeField] public Color StrokeColor { get; set; } = Color.red;
    [field:SerializeField] public float StrokeSize { get; set; } = 8;

    #endregion

    #region Project asset references

    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Public drawer methods

    public void ClearCanvas()
    {
        var prevRT = RenderTexture.active;
        RenderTexture.active = _rt;
        GL.Clear(false, true, Color.clear);
        RenderTexture.active = prevRT;
    }

    public void DrawLineSegment(Vector3 p0, Vector3 p1)
    {
        var prevRT = RenderTexture.active;
        RenderTexture.active = _rt;

        _material.SetVector("_Point0", p0);
        _material.SetVector("_Point1", p1);
        _material.SetColor("_Color", StrokeColor);
        _material.SetFloat("_Width", StrokeSize / Resolution);
        _material.SetFloat("_Aspect", (float)_rt.width / _rt.height);
        _material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Triangles, 12, 1);

        RenderTexture.active = prevRT;
    }

    #endregion

    #region Private members

    Scribble _ui;
    RenderTexture _rt;
    Material _material;

    bool LazyInitialize()
    {
        var rect = _ui.contentRect;
        if (float.IsNaN(rect.width) || rect.width < 8) return false;

        var height = (int)(Resolution * rect.height / rect.width);
        _rt = new RenderTexture(Resolution, height, 0);
        _rt.Create();

        ClearCanvas();
        _ui.style.backgroundImage = Background.FromRenderTexture(_rt);

        return true;
    }

    void DrawLineSegment((Vector3 p0, Vector3 p1)? seg)
    {
        if (seg == null) return;
        var rseg = ((Vector3 p0, Vector3 p1))seg;
        DrawLineSegment(rseg.p0, rseg.p1);
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _ui = root.Q<Scribble>(ElementID);

        _material = new Material(_shader);
        _material.hideFlags = HideFlags.DontSave;
    }

    void OnDestroy()
    {
        Destroy(_material);
        _material = null;

        if (_rt != null)
        {
            Destroy(_rt);
            _rt = null;
        }
    }

    void Update()
    {
        if (_rt == null && !LazyInitialize()) return;
        if (_ui.DequeueClearRequest()) ClearCanvas();
        DrawLineSegment(_ui.DequeueStroke());
    }

    #endregion
}

} // namespace Scrubber
