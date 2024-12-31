using System;
using Events;
using UnityEngine;

public class Level : MonoBehaviour {
    private EventProcessor<DeathEvent> _onDeathProcessor;

    void Awake() {
        _onDeathProcessor = new EventProcessor<DeathEvent>(GoToStart);
    }
    void OnEnable() => EventBus<DeathEvent>.Subscribe(_onDeathProcessor);
    void OnDisable() => EventBus<DeathEvent>.Unsubscribe(_onDeathProcessor);
    void GoToStart() => GameStateManager.Instance.MoveToState(GameStateManager.GameState.Start);
    
}
