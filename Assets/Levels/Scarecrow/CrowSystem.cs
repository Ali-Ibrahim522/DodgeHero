using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Levels.Scarecrow {
    public class CrowSystem : MonoBehaviour {
        [SerializeField] private GameObject crowPrefab;
        [SerializeField] private InputActionReference scareCrowKey;
    
        private List<CrowPerchingEvent> _perchingCrows;
    
        private int[] _spawnerIndexes;
        private int[] _spawnerChannels;
        private int _openSpawnerCount;
    
        private int _challengeCount;
        private int _activeCrows;
        private int _crowCap;
        private int _spawnRateLowerBound;
        private bool _disabled;

        private EventProcessor<CrowPerchingEvent> _onCrowPerchingEventProcessor;
        private EventProcessor<RemoveCrowEvent> _onRemoveCrowEventProcessor;
        private EventProcessor<DeathEventStatsUpdate> _onDeathEventStatsUpdateProcessor;

        private void Awake() {
            _onCrowPerchingEventProcessor = new EventProcessor<CrowPerchingEvent>(AddPerchingCrow);
            _onRemoveCrowEventProcessor = new EventProcessor<RemoveCrowEvent>(RemoveCrow);
            _onDeathEventStatsUpdateProcessor = new EventProcessor<DeathEventStatsUpdate>(DisableSystem);
        }
        
        void OnEnable() {
            scareCrowKey.action.started += OnScareCrowInput;
            _disabled = false;
            _challengeCount = 0;
            _activeCrows = 0;
            _crowCap = 2;
            _spawnRateLowerBound = 4;
            _perchingCrows = new List<CrowPerchingEvent>();
            _spawnerChannels = new int[6];
            _spawnerIndexes = new int[6];
            for (int i = 0; i < 6; i++) {
                _spawnerChannels[i] = i;
                _spawnerIndexes[i] = i;
            }
            _openSpawnerCount = 6;
            EventBus<CrowPerchingEvent>.Subscribe(_onCrowPerchingEventProcessor);
            EventBus<RemoveCrowEvent>.Subscribe(_onRemoveCrowEventProcessor);
            EventBus<DeathEventStatsUpdate>.Subscribe(_onDeathEventStatsUpdateProcessor);
        }

        void OnDisable() {
            scareCrowKey.action.started -= OnScareCrowInput;
            EventBus<CrowPerchingEvent>.Unsubscribe(_onCrowPerchingEventProcessor);
            EventBus<RemoveCrowEvent>.Unsubscribe(_onRemoveCrowEventProcessor);
            EventBus<DeathEventStatsUpdate>.Unsubscribe(_onDeathEventStatsUpdateProcessor);
        }
    
        private void OnScareCrowInput(InputAction.CallbackContext obj) {
            if (_perchingCrows.Count > 0) {
                ScareNearestCrow();
            } else {
                EventBus<MissEventHealthUpdate>.Publish(new MissEventHealthUpdate());
                EventBus<MissEventStatsUpdate>.Publish(new MissEventStatsUpdate());
            }
        }

        void Update() {
            if (_activeCrows < _crowCap && !_disabled) StartCoroutine(SpawnCrow());
        }

        private void DisableSystem() {
            StopAllCoroutines();
            _disabled = true;
        }

        private float GetNextCrowSpeed() => Mathf.Sqrt(.001f * _challengeCount) + .6f;

        private IEnumerator SpawnCrow() {
            _activeCrows++;
            float nextCrowSpeed = GetNextCrowSpeed();
            _challengeCount++;
            int crowSpawnerChannelIndex = Random.Range(0, _openSpawnerCount--);
            int crowSpawnerChannel = _spawnerChannels[crowSpawnerChannelIndex];
            SwapSpawnerChannelIndexes(crowSpawnerChannel, crowSpawnerChannelIndex, _spawnerChannels[_openSpawnerCount], _openSpawnerCount);
            yield return new WaitForSeconds(Random.Range(_spawnRateLowerBound, _spawnRateLowerBound + 3));
            EventBus<SpawnCrowEvent>.Publish(new SpawnCrowEvent {
                CrowPrefab = crowPrefab,
                CrowSpeed = nextCrowSpeed
            }, crowSpawnerChannel);
            if (_crowCap < 6 && _challengeCount % 20 == 0) _crowCap++;
            if (_spawnRateLowerBound > 1 && _challengeCount % 15 == 0) _spawnRateLowerBound--;
        }

        private void RemoveCrow(RemoveCrowEvent removeCrowEventProps) {
            for (int i = 0; i < _perchingCrows.Count; i++) {
                if (_perchingCrows[i].Channel == removeCrowEventProps.Channel) {
                    _activeCrows--;
                    SwapSpawnerChannelIndexes(
                        _perchingCrows[i].Channel,
                        _spawnerIndexes[_perchingCrows[i].Channel],
                        _spawnerChannels[_openSpawnerCount],
                        _spawnerIndexes[_spawnerChannels[_openSpawnerCount]]
                    );
                    _perchingCrows.RemoveAt(i);
                    _openSpawnerCount++;
                    return;
                }
            }
        }

        private void ScareNearestCrow() {
            _activeCrows--;
            int nearestCrowIndex = 0;
            for (int i = 1; i < _perchingCrows.Count; i++) {
                if (_perchingCrows[i].Progress.elapsed > _perchingCrows[nearestCrowIndex].Progress.elapsed) nearestCrowIndex = i;
            }
            EventBus<CrowScaredEvent>.Publish(new CrowScaredEvent(), _perchingCrows[nearestCrowIndex].Channel);
            SwapSpawnerChannelIndexes(
                _perchingCrows[nearestCrowIndex].Channel,
                _spawnerIndexes[_perchingCrows[nearestCrowIndex].Channel],
                _spawnerChannels[_openSpawnerCount],
                _spawnerIndexes[_spawnerChannels[_openSpawnerCount]]
            );
            _openSpawnerCount++;
            _perchingCrows.RemoveAt(nearestCrowIndex);
        }

        private void SwapSpawnerChannelIndexes(int sc1, int sci1, int sc2, int sci2) {
            _spawnerIndexes[sc1] = sci2;
            _spawnerIndexes[sc2] = sci1;
            _spawnerChannels[sci1] = sc2;
            _spawnerChannels[sci2] = sc1;
        }

        private void AddPerchingCrow(CrowPerchingEvent crowPerchingEventProps) => _perchingCrows.Add(crowPerchingEventProps);
    }
}
