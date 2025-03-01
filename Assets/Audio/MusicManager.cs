using Events;
using UnityEngine;

namespace Audio {
    public class MusicManager : MonoBehaviour {
        [SerializeField] private AudioSource as1;
        [SerializeField] private AudioSource as2;
        [SerializeField] private float crossFadeTime = 1f;
        [SerializeField] private float fadeAt = .5f;
        private float _fading;
        
        private AudioSource _current;
        private AudioSource _previous;
        
        private EventProcessor<PlayMusicEvent> _onPlayMusicEventProcessor;
        
        private string _musicID;
        private bool _persistDuration;

        private void Awake() {
            _current = as1;
            _previous = as2;
            _fading = 0;
            _persistDuration = false;
            _musicID = "";
            _onPlayMusicEventProcessor = new EventProcessor<PlayMusicEvent>(PlayMusic);
        }
        private void OnEnable() {
            EventBus<PlayMusicEvent>.Subscribe(_onPlayMusicEventProcessor);
        }
        private void OnDisable() {
            EventBus<PlayMusicEvent>.Unsubscribe(_onPlayMusicEventProcessor);
        }
        
        private void Update() => HandleCrossFade();
        
        private void PlayMusic(PlayMusicEvent playMusicEventProps) {
            _previous.Stop();
            (_previous, _current) = (_current, _previous);
            _current.volume = 0;
            _current.clip = playMusicEventProps.MusicData.musicClip;
            _persistDuration = playMusicEventProps.MusicData.musicID == _musicID;
            _musicID = playMusicEventProps.MusicData.musicID;
            _fading = .001f;
            if (!_previous.isPlaying) {
                InitCurrent();
                _fading += fadeAt * crossFadeTime;
            }
        }

        private void InitCurrent() {
            _current.Play();
            _current.time = _persistDuration && _previous.isPlaying ? _previous.time : 0;
        }

        private void HandleCrossFade() {
            if (_fading <= 0f) return;
            _fading += Time.deltaTime;
            float progress = Mathf.Clamp(_fading / crossFadeTime, 0, 1 + fadeAt);
            if (_previous.isPlaying) {
                _previous.volume = 1.0f - Mathf.Log10(1 + 9 * progress) / Mathf.Log10(10);
                if (progress >= fadeAt && !_current.isPlaying) InitCurrent();
                if (progress >= 1) _previous.Stop();
            }
            if (_current.isPlaying) {
                _current.volume = Mathf.Log10(1 + 9 * (progress - fadeAt)) / Mathf.Log10(10);
                if (progress - fadeAt >= 1) _fading = 0f;
            }
        }
    }
}
