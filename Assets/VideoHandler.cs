using UnityEngine;
using Klak.Hap;
using Klak.Math;

namespace Scrubber
{
    public sealed class VideoHandler : MonoBehaviour
    {
        [SerializeField] float _wheelSpeed = 1;
        [SerializeField] float _tweenSpeed = 5;

        HapPlayer _player;
        float _time;
        CdsTween _tween;

        public void Open(string name)
        {
            _player = GetComponent<HapPlayer>();
            _player.Open(name + ".mov");
        }

        void Update()
        {
            if (_player == null) return;

            _time += Input.mouseScrollDelta.y * _wheelSpeed;
            _time = Mathf.Clamp(_time, 0, (float)_player.streamDuration);

            _tween.Speed = _tweenSpeed;
            _tween.Step(_time);

            _player.time = _tween.Current;
        }
    }
}
