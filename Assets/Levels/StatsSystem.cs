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
        private EventProcessor<HitEvent> _onHitProcessor;
        private EventProcessor<MissEventStatsUpdate> _onMissProcessor;
        private EventProcessor<DeathEventStatsUpdate> _onDeathProcessor;

        private int _maxCombo;
        private float _time;
        private int _totalHits;
        
        void Awake() {
            _onDeathProcessor = new EventProcessor<DeathEventStatsUpdate>(LoadResultsData);
            _onHitProcessor = new EventProcessor<HitEvent>(IncreaseScore);
            _onMissProcessor = new EventProcessor<MissEventStatsUpdate>(ResetCombo);
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
            EventBus<DeathEventStatsUpdate>.Subscribe(_onDeathProcessor);
            EventBus<MissEventStatsUpdate>.Subscribe(_onMissProcessor);
            EventBus<HitEvent>.Subscribe(_onHitProcessor);
        }
        void OnDisable() {
            EventBus<DeathEventStatsUpdate>.Unsubscribe(_onDeathProcessor);
            EventBus<MissEventStatsUpdate>.Unsubscribe(_onMissProcessor);
            EventBus<HitEvent>.Unsubscribe(_onHitProcessor);
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