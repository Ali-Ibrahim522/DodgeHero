using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Audio {
    public class SoundEffectManager : MonoBehaviour {
        [SerializeField] private SoundEffectPlayer playerPrefab;
        [SerializeField] private int startingPoolSize = 10;
        [SerializeField] private int maxPoolSize = 20;
        
        private IObjectPool<SoundEffectPlayer> _players;
        private EventProcessor<ReleaseAudioPlayerEvent> _onReleaseAudioPlayerEventProcessor;
        private EventProcessor<PlayAudioEvent> _onPlayAudioEventProcessor;
        private readonly Dictionary<SoundEffectData, int> _activeCount = new ();

        private void Awake() {
            _onReleaseAudioPlayerEventProcessor = new EventProcessor<ReleaseAudioPlayerEvent>(ReturnAudioPlayer);
            _onPlayAudioEventProcessor = new EventProcessor<PlayAudioEvent>(PlayAudio);
            _players = new ObjectPool<SoundEffectPlayer>(
                InitAudioPlayer,
                GetAudioPlayer,
                ReleaseAudioPlayer,
                DestroyAudioPlayer,
                true,
                startingPoolSize,
                maxPoolSize
                );
        }

        private void OnEnable() {
            EventBus<ReleaseAudioPlayerEvent>.Subscribe(_onReleaseAudioPlayerEventProcessor);
            EventBus<PlayAudioEvent>.Subscribe(_onPlayAudioEventProcessor);
        }

        private void OnDisable() {
            EventBus<ReleaseAudioPlayerEvent>.Unsubscribe(_onReleaseAudioPlayerEventProcessor);
            EventBus<PlayAudioEvent>.Unsubscribe(_onPlayAudioEventProcessor);
        }

        private void ReturnAudioPlayer(ReleaseAudioPlayerEvent releaseAudioPlayerEventProps) {
            _players.Release(releaseAudioPlayerEventProps.SoundEffectPlayer);
        }

        private void PlayAudio(PlayAudioEvent playAudioEventProps) {
            if (_activeCount.TryGetValue(playAudioEventProps.SoundEffectData, out int count)) {
                if (count >= playAudioEventProps.SoundEffectData.maxCount) return;
                _activeCount[playAudioEventProps.SoundEffectData] = count + 1;
            } else {
                _activeCount[playAudioEventProps.SoundEffectData] = 1;
            }
            _players.Get().Play(playAudioEventProps.SoundEffectData);
        }

        private void DestroyAudioPlayer(SoundEffectPlayer player) => Destroy(player);
        private void ReleaseAudioPlayer(SoundEffectPlayer player) {
            _activeCount[player.sfxData]--;
            player.gameObject.SetActive(false);
        }
        private void GetAudioPlayer(SoundEffectPlayer player) {
            player.gameObject.SetActive(true);
        }
        private SoundEffectPlayer InitAudioPlayer() {
            SoundEffectPlayer player = Instantiate(playerPrefab, transform);
            player.gameObject.SetActive(false);
            return player;
        }
    }
}
