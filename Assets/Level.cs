using Events;
using UnityEngine;

public class Level : MonoBehaviour {
    private EventProcessor<DeathEvent> _onDeathProcessor;

    void Awake() {
        _onDeathProcessor = new EventProcessor<DeathEvent>(GoToResults);
    }
    void OnEnable() => EventBus<DeathEvent>.Subscribe(_onDeathProcessor);
    void OnDisable() => EventBus<DeathEvent>.Unsubscribe(_onDeathProcessor);
    void GoToResults() => GameStateManager.Instance.MoveToState(GameStateManager.GameState.Results);
    
}
