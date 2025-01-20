using Events;
using UnityEngine;

public class CrowSpawner : MonoBehaviour {
    [SerializeField] private int channel;
    [SerializeField] private CubicPath crowPath;
    [SerializeField] private Quaternion crowRotation;
    private EventProcessor<CrowDeathEvent> _onCrowDeathEvent;
    private EventProcessor<SpawnCrowEvent> _onSpawnCrowEvent;
    
    public void Awake() {
        _onCrowDeathEvent = new EventProcessor<CrowDeathEvent>(OpenSpawner);
        _onSpawnCrowEvent = new EventProcessor<SpawnCrowEvent>(SpawnCrow);
    }

    public void OnEnable() {
        OpenSpawner();
        EventBus<CrowDeathEvent>.Subscribe(_onCrowDeathEvent, channel);    
    }

    public void OnDisable() {
        EventBus<CrowDeathEvent>.Unsubscribe(_onCrowDeathEvent, channel);
        CloseSpawner();
    }

    public void OpenSpawner() => EventBus<SpawnCrowEvent>.Subscribe(_onSpawnCrowEvent);
    
    public void CloseSpawner() => EventBus<SpawnCrowEvent>.Unsubscribe(_onSpawnCrowEvent);
    
    public void SpawnCrow(SpawnCrowEvent crowEventProps) {
        if (crowEventProps.CrowCountdown == 0) {
            CloseSpawner();
            Instantiate(crowEventProps.CrowPrefab, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<CrowChallenge>().InitCrow(channel, crowPath, crowEventProps.CrowSpeed, crowRotation);
        }
        crowEventProps.CrowCountdown--;
    }
}