using UnityEngine;
using UnityEngine.UIElements;

namespace Scrubber {

[UxmlElement]
public partial class Scribble : VisualElement
{
    #region Visual element implementation

    public static readonly string ussClassName = "scribble";

    public Scribble()
    {
        AddToClassList(ussClassName);
        this.AddManipulator(_logger = new InputLogger(this));
    }

    #endregion

    #region Private members

    InputLogger _logger;

    Vector2 TransformPoint(Vector2 p)
    {
        var r = contentRect;
        return new Vector2((p.x - r.x) / r.width  * 2 - 1,
                           (p.y - r.y) / r.height * 2 - 1);
    }

    (Vector2, Vector2) TransformPoints(Vector2 p1, Vector2 p2)
      => (TransformPoint(p1), TransformPoint(p2));

    #endregion

    #region Input data accessors

    public bool DequeueClearRequest()
    {
        var queue = _logger.EventQueue;
        if (queue.Count == 0 || !queue.Peek().IsClear) return false;
        queue.Dequeue();
        return true;
    }

    public (Vector2 p1, Vector2 p2)? DequeueStroke()
    {
        var queue = _logger.EventQueue;

        if (queue.Count < 2) return null;

        var p1 = queue.Dequeue().Coords;
        while (queue.Count > 1)
        {
            var evt = queue.Dequeue();
            if (evt.IsUp) return TransformPoints(p1, evt.Coords);
        }

        var end = queue.Peek();
        if (end.IsUp) queue.Dequeue();

        return TransformPoints(p1, end.Coords);
    }

    #endregion
}

} // namespace Scrubber
