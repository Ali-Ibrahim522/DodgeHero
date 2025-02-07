using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Levels
{
    public class StatsSystem : MonoBehaviour {
        [SerializeField] private Sprite resultsSplash;
        private int _combo;
        private long _score;
        public TMP_Text comboText;
        public TMP_Text scoreText;

        private int _maxCombo;
        private float _time;
        private int _totalHits;
        
        private EventProcessor<DeathEventStatsUpdate> _onDeathEventStatsUpdateProcessor;
        private EventProcessor<MissEventStatsUpdate> _onMissEventHealthUpdateProcessor;
        private EventProcessor<HitEvent> _onHitEventProcessor;

        private void Awake() {
            _onDeathEventStatsUpdateProcessor = new EventProcessor<DeathEventStatsUpdate>(LoadResultsData);
            _onMissEventHealthUpdateProcessor = new EventProcessor<MissEventStatsUpdate>(ResetCombo);
            _onHitEventProcessor = new EventProcessor<HitEvent>(IncreaseScore);
        }
        void Update() => _time += Time.deltaTime;
        void OnEnable() {
            _combo = 0;
            _score = 0;
            _time = 0;
            _maxCombo = 0;
            _totalHits = 0;
            comboText.text = "";
            scoreText.text = "";
            EventBus<DeathEventStatsUpdate>.Subscribe(_onDeathEventStatsUpdateProcessor);
            EventBus<MissEventStatsUpdate>.Subscribe(_onMissEventHealthUpdateProcessor);
            EventBus<HitEvent>.Subscribe(_onHitEventProcessor);
        }
        void OnDisable() {
            EventBus<DeathEventStatsUpdate>.Unsubscribe(_onDeathEventStatsUpdateProcessor);
            EventBus<MissEventStatsUpdate>.Unsubscribe(_onMissEventHealthUpdateProcessor);
            EventBus<HitEvent>.Unsubscribe(_onHitEventProcessor);
        }

        void ResetCombo() {
            _totalHits += _combo;
            _maxCombo = Mathf.Max(_combo, _maxCombo);
            _combo = 0;
            comboText.text = $"{_combo}x";
        }

        void LoadResultsData() => EventBus<LoadResultsDataEvent>.Publish(new LoadResultsDataEvent {
            ResultsSprite = resultsSplash,
            Score = _score,
            MaxCombo = _maxCombo,
            TotalHits = _totalHits,
            Time = _time
        });
        
        void IncreaseScore(HitEvent hitEventProps) {
            _score += ++_combo * hitEventProps.Gained;
            scoreText.text = _score.ToString();
            comboText.text = $"{_combo}x";
        }
    }
}