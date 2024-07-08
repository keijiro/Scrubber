using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace Scrubber {

sealed class InputLogger : PointerManipulator
{
    #region Input event queue

    public Queue<InputEvent> EventQueue { get; private set; }
      = new Queue<InputEvent>();

    #endregion

    #region Private variables

    int _pointerID;

    bool IsActive => _pointerID >= 0;

    #endregion

    #region PointerManipulator implementation

    public InputLogger(Scribble scribble)
    {
        _pointerID = -1;
        activators.Add(new ManipulatorActivationFilter{button = MouseButton.LeftMouse});
        activators.Add(new ManipulatorActivationFilter{button = MouseButton.RightMouse});
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        target.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
    }

    #endregion

    #region Pointer callbacks

    void OnPointerDown(PointerDownEvent e)
    {
        if (IsActive)
        {
            e.StopImmediatePropagation();
        }
        else if (CanStartManipulation(e))
        {
            if (e.button == 0)
            {
            EventQueue.Enqueue(InputEvent.NewDown(e.localPosition));
            target.CapturePointer(_pointerID = e.pointerId);
            }
            else
            {
                EventQueue.Enqueue(InputEvent.NewClear(e.localPosition));
            }
            e.StopPropagation();
        }
    }

    void OnPointerMove(PointerMoveEvent e)
    {
        if (!IsActive || !target.HasPointerCapture(_pointerID)) return;
        EventQueue.Enqueue(InputEvent.NewMove(e.localPosition));
        e.StopPropagation();
    }

    void OnPointerUp(PointerUpEvent e)
    {
        if (!IsActive || !target.HasPointerCapture(_pointerID)) return;

        if (CanStopManipulation(e))
        {
            EventQueue.Enqueue(InputEvent.NewUp(e.localPosition));
            _pointerID = -1;
            target.ReleaseMouse();
            e.StopPropagation();
        }
    }

    #endregion
}

} // namespace Scrubber
