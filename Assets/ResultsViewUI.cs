using Events;
using UnityEngine;

public class ResultsViewUI : MonoBehaviour {
    public void OnEnable() {
        if (!GameStateManager.Instance.IsPlayerGuest()) EventBus<LoadLeaderboardEvent>.Publish(new LoadLeaderboardEvent());
    }
}