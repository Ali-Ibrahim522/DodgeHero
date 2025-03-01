using Global;
using PlayFab;
using UnityEngine;

namespace Login {
    public class LogoutButtonController : BasicButtonController {
        public void OnLogoutButtonClicked() {
            PlayFabClientAPI.ForgetAllCredentials();
            PlayerAuthManager.Logout();
            PlayerDataManager.Instance.ResetPlayerData();
            OnClickToNextState();
        }
    }
}