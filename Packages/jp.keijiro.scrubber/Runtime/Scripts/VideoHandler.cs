using UnityEngine;
using UnityEngine.InputSystem;
using Klak.Hap;
using Klak.Math;

namespace Scrubber {

public sealed class VideoHandler : MonoBehaviour
{
    #region Public properties

    public float WheelSpeed { get; set; } = 1;
    public float TweenSpeed { get; set; } = 8;

    #endregion

    #region Package asset references

    [SerializeField, HideInInspector] Material _hapMaterial = null;
    [SerializeField, HideInInspector] Material _hapQMaterial = null;

    #endregion

    #region Internal state

    HapPlayer _player;
    Renderer _renderer;
    float _time;
    (float x, float v) _tween;

    #endregion

    #region Public methods

    public void Open(string name, bool autoPlay, bool loop)
    {
        _player = GetComponent<HapPlayer>();
        _renderer = GetComponent<Renderer>();

        _player.Open(name + ".mov");
        _player.speed = autoPlay ? 1 : 0;
        _player.loop = loop;
    }

    #endregion

    #region MonoBehaviour implementation

    void Update()
    {
        if (_player == null) return;

        var wheel = Mouse.current.scroll.x.value;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            _player.speed = 1 - _player.speed;

        if (wheel == 0 && _player.speed == 1)
        {
            _tween.x = _time = _player.time;
             return;
        }

        _time += wheel * WheelSpeed / 60;

        if (!_player.loop)
            _time = Mathf.Clamp(_time, 0, (float)_player.streamDuration);

        _tween = CdsTween.Step(_tween, _time, TweenSpeed);

        _player.time = _tween.x;
        _player.speed = 0;

        _renderer.sharedMaterial =
          _player.codecType == CodecType.HapQ ? _hapQMaterial : _hapMaterial;
    }

    #endregion
}

} // namespace Scrubber
