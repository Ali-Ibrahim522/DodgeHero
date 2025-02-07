using Events;
using UnityEngine;

namespace Levels.Scarecrow {
    [System.Serializable]
    public class TargetChallenge : MonoBehaviour {
        [SerializeField] private GameObject target;
        [SerializeField] private SpriteRenderer targetIndicator;
        [SerializeField] private GameObject stump;
        public bool done;
        
        private EventProcessor<DeathEventStatsUpdate> _onDeathEventStatsUpdateProcessor;

        private void Awake() => _onDeathEventStatsUpdateProcessor = new EventProcessor<DeathEventStatsUpdate>(DisableChallenge);

        void OnEnable() {
            EventBus<DeathEventStatsUpdate>.Subscribe(_onDeathEventStatsUpdateProcessor);
            done = false;
        }

        void OnDisable() => EventBus<DeathEventStatsUpdate>.Unsubscribe(_onDeathEventStatsUpdateProcessor);

        public bool OnTargetHit() {
            if (done) return false;
            done = true;
            return true;
        }
        
        private void DisableChallenge() => done = true;
        
        public void SetTargetHit(Color hit) {
            EventBus<TargetHitEvent>.Publish(new TargetHitEvent());
            targetIndicator.color = hit;
            done = true;
        }

        public void SetTargetMissed(Color missed) {
            targetIndicator.color = missed;
            done = true;
        }

        public void SetTargetProposed(Color proposed) {
            targetIndicator.color = proposed;
            stump.SetActive(false);
            target.SetActive(true);
        }

        public void SetStump() {
            targetIndicator.color = Color.white;
            target.SetActive(false);
            stump.SetActive(true);
        }
    }
}
