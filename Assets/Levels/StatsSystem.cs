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
            _onHitProcessor = new EventProcessor<HitEvent>(IncreaseScore);
            _onMissProcessor = new EventProcessor<MissEventStatsUpdate>(ResetCombo);
        }
        void OnEnable() {
            _combo = 0;
            _score = 0;
            comboText.text = "";
            scoreText.text = "";
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
        
        void IncreaseScore(HitEvent hitEventProps) {
            _score += ++_combo * hitEventProps.Gained;
            scoreText.text = _score.ToString();
            comboText.text = $"{_combo}x";
        }
    }
}