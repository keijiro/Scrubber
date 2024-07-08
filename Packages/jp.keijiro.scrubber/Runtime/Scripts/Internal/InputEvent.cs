using UnityEngine;

namespace Scrubber {

public readonly struct InputEvent
{
    #region Public accessors

    public readonly Vector2 Coords { get; }

    public bool IsDown  => _mode == Mode.Down;
    public bool IsMove  => _mode == Mode.Move;
    public bool IsUp    => _mode == Mode.Up;
    public bool IsClear => _mode == Mode.Clear;

    #endregion

    #region Factory methods

    public static InputEvent NewDown(Vector2 coords)
      => new InputEvent(Mode.Down, coords);

    public static InputEvent NewMove(Vector2 coords)
      => new InputEvent(Mode.Move, coords);

    public static InputEvent NewUp(Vector2 coords)
      => new InputEvent(Mode.Up, coords);

    public static InputEvent NewClear(Vector2 coords)
      => new InputEvent(Mode.Clear, coords);

    #endregion

    #region Private members

    enum Mode { Down, Move, Up, Clear }

    readonly Mode _mode;

    InputEvent(Mode mode, Vector2 coords)
      => (_mode, Coords) = (mode, coords);

    #endregion
}

} // namespace Scrubber
