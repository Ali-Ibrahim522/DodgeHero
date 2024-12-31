using Events;
using TMPro;
using UnityEngine;

namespace Levels
{
    public class StatsSystem : MonoBehaviour
    {
        private int _combo;
        private long _score;
        public TMP_Text comboText;
        public TMP_Text scoreText;
        private EventProcessor<HitEvent> _onHitProcessor;
        private EventProcessor<MissEventStatsUpdate> _onMissProcessor;
        void Awake() {
            comboText.text = "";
            scoreText.text = "";
            _onHitProcessor = new EventProcessor<HitEvent>(IncrementCombo);
            _onHitProcessor.Add(IncreaseScore);
            _onMissProcessor = new EventProcessor<MissEventStatsUpdate>(ResetCombo);
        }
        void OnEnable() {
            _combo = 0;
            _score = 0;
            EventBus<MissEventStatsUpdate>.Subscribe(_onMissProcessor);
            EventBus<HitEvent>.Subscribe(_onHitProcessor);
        }
        void OnDisable() {
            EventBus<MissEventStatsUpdate>.Unsubscribe(_onMissProcessor);
            EventBus<HitEvent>.Unsubscribe(_onHitProcessor);
        }

        void ResetCombo() {
            _combo = 0;
            comboText.text = $"{_combo}x";
        }
        void IncrementCombo() {
            _combo++;
            comboText.text = $"{_combo}x";
        }
        void IncreaseScore(HitEvent hitEventProps) {
            _score += hitEventProps.Gained;
            scoreText.text = _score.ToString();
        }
    }
}