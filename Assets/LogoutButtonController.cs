using PlayFab;
using UnityEngine;

public class LogoutButtonController : MonoBehaviour {
    public void OnLogoutButtonClicked() {
        PlayFabClientAPI.ForgetAllCredentials();
        GameStateManager.Instance.ResetPlayerDetails();
        GameStateManager.Instance.MoveToState(GameStateManager.GameState.Login);
    }
}