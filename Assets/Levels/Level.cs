using Audio;
using Events;
using Global;
using UnityEngine;

namespace Levels {
    public class Level : MonoBehaviour {
        [SerializeField] private SoundEffectData hitSound;
        [SerializeField] private SoundEffectData missSound;
        [SerializeField] private MusicData levelMusic;
        
        private EventProcessor<DeathEvent> _onDeathEventProcessor;
        private EventProcessor<MissEventStatsUpdate> _onMissEventStatsUpdateProcessor;
        private EventProcessor<HitEvent> _onHitEventProcessor;

        private void Awake() {
            _onHitEventProcessor = new EventProcessor<HitEvent>(PlayHitSound);
            _onMissEventStatsUpdateProcessor = new EventProcessor<MissEventStatsUpdate>(PlayMissSound);
            _onDeathEventProcessor = new EventProcessor<DeathEvent>(GoToResults);
        }
        private void OnEnable() {
            hitSound.clip.LoadAudioData();
            missSound.clip.LoadAudioData();
            EventBus<DeathEvent>.Subscribe(_onDeathEventProcessor);
            EventBus<MissEventStatsUpdate>.Subscribe(_onMissEventStatsUpdateProcessor);
            EventBus<HitEvent>.Subscribe(_onHitEventProcessor);
            EventBus<PlayMusicEvent>.Publish(new PlayMusicEvent {
                MusicData = levelMusic
            });
        }
        private void OnDisable() {
            hitSound.clip.UnloadAudioData();
            missSound.clip.UnloadAudioData();
            EventBus<DeathEvent>.Unsubscribe(_onDeathEventProcessor);
            EventBus<MissEventStatsUpdate>.Unsubscribe(_onMissEventStatsUpdateProcessor);
            EventBus<HitEvent>.Unsubscribe(_onHitEventProcessor);
        }
        private void GoToResults() => GameStateManager.Instance.MoveToState(GameStateManager.GameState.Results);

        private void PlayHitSound() {
            EventBus<PlayAudioEvent>.Publish(new PlayAudioEvent { SoundEffectData = hitSound });
        }
        private void PlayMissSound() => EventBus<PlayAudioEvent>.Publish(new PlayAudioEvent { SoundEffectData = missSound });
    }
}
