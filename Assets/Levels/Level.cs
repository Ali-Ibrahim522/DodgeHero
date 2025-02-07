using Events;
using Global;
using UnityEngine;

namespace Levels {
    public class Level : MonoBehaviour {
        private EventProcessor<DeathEvent> _onDeathEventProcessor;

        private void Awake() => _onDeathEventProcessor = new EventProcessor<DeathEvent>(GoToResults);
        private void OnEnable() => EventBus<DeathEvent>.Subscribe(_onDeathEventProcessor);
        private void OnDisable() => EventBus<DeathEvent>.Unsubscribe(_onDeathEventProcessor);
        private void GoToResults() => GameStateManager.Instance.MoveToState(GameStateManager.GameState.Results);
    
    }
}
