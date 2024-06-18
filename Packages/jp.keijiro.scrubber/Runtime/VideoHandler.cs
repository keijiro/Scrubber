using UnityEngine;
using UnityEngine.InputSystem;
using Klak.Hap;
using Klak.Math;

namespace Scrubber
{
    public sealed class VideoHandler : MonoBehaviour
    {
        [field:SerializeField] public float WheelSpeed = 0.01f;
        [field:SerializeField] public float TweenSpeed = 8;

        [SerializeField, HideInInspector] Material _hapMaterial = null;
        [SerializeField, HideInInspector] Material _hapQMaterial = null;

        HapPlayer _player;
        Renderer _renderer;
        float _time;
        (float x, float v) _tween;

        public void Open(string name, bool autoPlay, bool loop)
        {
            _player = GetComponent<HapPlayer>();
            _renderer = GetComponent<Renderer>();

            _player.Open(name + ".mov");
            _player.speed = autoPlay ? 1 : 0;
            _player.loop = loop;
        }

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

            _time += wheel * WheelSpeed;

            if (!_player.loop)
                _time = Mathf.Clamp(_time, 0, (float)_player.streamDuration);

            _tween = CdsTween.Step(_tween, _time, TweenSpeed);

            _player.time = _tween.x;
            _player.speed = 0;

            _renderer.sharedMaterial =
              _player.codecType == CodecType.HapQ ? _hapQMaterial : _hapMaterial;
        }
    }
}
