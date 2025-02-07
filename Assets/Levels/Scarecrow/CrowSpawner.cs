using Events;
using UnityEngine;

namespace Levels.Scarecrow {
    public class CrowSpawner : MonoBehaviour {
        [SerializeField] private int channel;
        [SerializeField] private CubicPath crowPath;
        [SerializeField] private Quaternion crowRotation;

        private EventProcessor<SpawnCrowEvent> _onSpawnCrowEventProcessor;

        private void Awake() => _onSpawnCrowEventProcessor = new EventProcessor<SpawnCrowEvent>(SpawnCrow);
        
        private void OnEnable() => EventBus<SpawnCrowEvent>.Subscribe(_onSpawnCrowEventProcessor, channel);

        private void OnDisable() => EventBus<SpawnCrowEvent>.Unsubscribe(_onSpawnCrowEventProcessor, channel);
    
        private void SpawnCrow(SpawnCrowEvent crowEventProps) { 
            Instantiate(crowEventProps.CrowPrefab, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<CrowChallenge>().InitCrow(channel, crowPath, crowEventProps.CrowSpeed, crowRotation);
        }
    }
}