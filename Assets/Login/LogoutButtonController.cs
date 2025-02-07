using Global;
using PlayFab;
using UnityEngine;

namespace Login {
    public class LogoutButtonController : MonoBehaviour {
        public void OnLogoutButtonClicked() {
            PlayFabClientAPI.ForgetAllCredentials();
            PlayerAuthManager.Logout();
            PlayerDataManager.Instance.ResetPlayerData();
            GameStateManager.Instance.MoveToState(GameStateManager.GameState.Login);
        }
    }
}