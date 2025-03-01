using System.Collections;
using Events;
using UnityEngine;

namespace Audio {
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffectPlayer : MonoBehaviour {
        public SoundEffectData sfxData;
        private AudioSource _source;

        private void Awake() => _source = gameObject.GetComponent<AudioSource>();
        
        private IEnumerator ReleaseWhenDone() {
            yield return new WaitWhile(() => _source.isPlaying);
            EventBus<ReleaseAudioPlayerEvent>.Publish(new ReleaseAudioPlayerEvent {
                SoundEffectPlayer = this
            });
        }

        public void Play(SoundEffectData data) {
            sfxData = data;
            _source.clip = data.clip;
            _source.pitch = Random.Range(1 - data.pitchVariance, 1 + data.pitchVariance);
            _source.volume = data.volume;
            _source.Play();
            StartCoroutine(ReleaseWhenDone());
        }
    }
}
