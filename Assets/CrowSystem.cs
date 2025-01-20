using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrowSystem : MonoBehaviour {
    [SerializeField] private GameObject crowPrefab;
    private List<CrowPerchingEvent> _perchingCrows;
    private EventProcessor<CrowPerchingEvent> _onCrowPerchingEvent;
    private EventProcessor<CrowMissedEvent> _onCrowMissedEvent;
    private int _challengeCount;
    private int _activeCrows;
    private int _crowCap;
    private int _spawnRateLowerBound;
    
    void Awake() {
        _onCrowPerchingEvent = new EventProcessor<CrowPerchingEvent>(AddPerchingCrow);
        _onCrowMissedEvent = new EventProcessor<CrowMissedEvent>(RemoveCrow);
    }

    void OnEnable() {
        _challengeCount = 0;
        _activeCrows = 0;
        _crowCap = 2;
        _spawnRateLowerBound = 5;
        _perchingCrows = new();
        EventBus<CrowPerchingEvent>.Subscribe(_onCrowPerchingEvent);
        EventBus<CrowMissedEvent>.Subscribe(_onCrowMissedEvent);
    }

    void OnDisable() {
        EventBus<CrowPerchingEvent>.Unsubscribe(_onCrowPerchingEvent);
        EventBus<CrowMissedEvent>.Unsubscribe(_onCrowMissedEvent);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (_perchingCrows.Count > 0) {
                ScareNearestCrow();
            } else {
                EventBus<MissEventHealthUpdate>.Publish(new MissEventHealthUpdate(.75f));
                EventBus<MissEventStatsUpdate>.Publish(new MissEventStatsUpdate());
            }
        }
        if (_activeCrows < _crowCap) StartCoroutine(SpawnCrow());
    }

    public float GetNextCrowSpeed() => Mathf.Sqrt(.001f * _challengeCount) + .6f;

    private IEnumerator SpawnCrow() {
        _activeCrows++;
        float nextCrowSpeed = GetNextCrowSpeed();
        _challengeCount++;
        yield return new WaitForSeconds(Random.Range(_spawnRateLowerBound, _spawnRateLowerBound + 3));
        EventBus<SpawnCrowEvent>.Publish(new SpawnCrowEvent(nextCrowSpeed, Random.Range(0, EventBus<SpawnCrowEvent>.SubscriberCount()), crowPrefab));
        if (_crowCap < 6 && _challengeCount % 20 == 0) _crowCap++;
        if (_spawnRateLowerBound > 1 && _challengeCount % 15 == 0) _spawnRateLowerBound--;
    }

    private void RemoveCrow(CrowMissedEvent crowMissedEventProps) {
        _activeCrows--;
        EventBus<MissEventHealthUpdate>.Publish(new MissEventHealthUpdate(.75f));
        EventBus<MissEventStatsUpdate>.Publish(new MissEventStatsUpdate());
        for (int i = 0; i < _perchingCrows.Count; i++) {
            if (_perchingCrows[i].Channel == crowMissedEventProps.Channel) {
                _perchingCrows.RemoveAt(i);
                return;
            }
        }
    }

    private void ScareNearestCrow() {
        _activeCrows--;
        float nearestCrowDistance = Vector3.Distance(_perchingCrows[0].CrowPosition.position, _perchingCrows[0].Dest);
        int nearestCrowIndex = 0;
        for (int i = 1; i < _perchingCrows.Count; i++) {
            float currentCrowDistance = Vector3.Distance(_perchingCrows[i].CrowPosition.position, _perchingCrows[i].Dest);
            if (currentCrowDistance < nearestCrowDistance) {
                nearestCrowIndex = i;
                nearestCrowDistance = currentCrowDistance;
            }
        }
        _perchingCrows[nearestCrowIndex].OnCrowPerching();
        _perchingCrows.RemoveAt(nearestCrowIndex);
    }

    public void AddPerchingCrow(CrowPerchingEvent crowPerchingEventProps) {
        _perchingCrows.Add(crowPerchingEventProps); 
    }
}
