using Events;
using UnityEngine;

public class CrowSpawner : MonoBehaviour {
    [SerializeField] private int channel;
    [SerializeField] private CubicPath crowPath;
    [SerializeField] private Quaternion crowRotation;
    private EventProcessor<SpawnCrowEvent> _onSpawnCrowEvent;
    
    public void Awake() => _onSpawnCrowEvent = new EventProcessor<SpawnCrowEvent>(SpawnCrow);

    public void OnEnable() => EventBus<SpawnCrowEvent>.Subscribe(_onSpawnCrowEvent, channel);

    public void OnDisable() => EventBus<SpawnCrowEvent>.Unsubscribe(_onSpawnCrowEvent, channel);
    
    private void SpawnCrow(SpawnCrowEvent crowEventProps) { 
        Instantiate(crowEventProps.CrowPrefab, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<CrowChallenge>().InitCrow(channel, crowPath, crowEventProps.CrowSpeed, crowRotation);
    }
}