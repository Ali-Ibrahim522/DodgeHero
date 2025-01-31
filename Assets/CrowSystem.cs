using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrowSystem : MonoBehaviour {
    [SerializeField] private GameObject crowPrefab;
    private List<CrowPerchingEvent> _perchingCrows;
    
    private int[] _spawnerIndexes;
    private int[] _spawnerChannels;
    private int _openSpawnerCount;
    
    private EventProcessor<CrowPerchingEvent> _onCrowPerchingEvent;
    private EventProcessor<RemoveCrowEvent> _onCrowMissedEvent;
    private EventProcessor<DeathEventStatsUpdate> _onDeathEventProcessor;
    
    private int _challengeCount;
    private int _activeCrows;
    private int _crowCap;
    private int _spawnRateLowerBound;
    private float _systemTimespan;
    private bool _disabled;
    void Awake() {
        _onCrowPerchingEvent = new EventProcessor<CrowPerchingEvent>(AddPerchingCrow);
        _onCrowMissedEvent = new EventProcessor<RemoveCrowEvent>(RemoveCrow);
        _onDeathEventProcessor = new EventProcessor<DeathEventStatsUpdate>(DisableSystem);
    }

    void OnEnable() {
        _disabled = false;
        _systemTimespan = 0;
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
        EventBus<CrowPerchingEvent>.Subscribe(_onCrowPerchingEvent);
        EventBus<RemoveCrowEvent>.Subscribe(_onCrowMissedEvent);
        EventBus<DeathEventStatsUpdate>.Subscribe(_onDeathEventProcessor);
    }

    void OnDisable() {
        EventBus<CrowPerchingEvent>.Unsubscribe(_onCrowPerchingEvent);
        EventBus<RemoveCrowEvent>.Unsubscribe(_onCrowMissedEvent);
        EventBus<DeathEventStatsUpdate>.Unsubscribe(_onDeathEventProcessor);
    }

    void Update() {
        _systemTimespan += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (_perchingCrows.Count > 0) {
                ScareNearestCrow();
            } else {
                EventBus<MissEventHealthUpdate>.Publish(new MissEventHealthUpdate {
                    DeathWait = .75f
                });
                EventBus<MissEventStatsUpdate>.Publish(new MissEventStatsUpdate());
            }
        }
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
            if (_perchingCrows[i].DestTime < _perchingCrows[nearestCrowIndex].DestTime) nearestCrowIndex = i;
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

    private void AddPerchingCrow(CrowPerchingEvent crowPerchingEventProps) {
        _perchingCrows.Add(new CrowPerchingEvent {
            Channel = crowPerchingEventProps.Channel,
            DestTime = _systemTimespan + crowPerchingEventProps.DestTime,
        }); 
    }
}
