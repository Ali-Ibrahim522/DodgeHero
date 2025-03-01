using System.Collections.Generic;
using Global;
using PlayFab;
using PlayFab.AdminModels;
using PlayFab.ProgressionModels;
using TMPro;
using UnityEngine;
using EntityKey = PlayFab.ProgressionModels.EntityKey;

namespace PlayerSettings {
    public class DeleteAccountHandler : MonoBehaviour {
        [SerializeField] private TMP_Text deleteAccountErrorText;
        [SerializeField] private GameObject deleteAccountConfirmationContainer;

        public void OnEnable() {
            deleteAccountErrorText.text = "";
            deleteAccountConfirmationContainer.SetActive(false);
        }

        public void OnConfirmClicked() {
            PlayFabProgressionAPI.DeleteStatistics(new DeleteStatisticsRequest {
                Entity = new EntityKey {
                    Id = PlayFabSettings.staticPlayer.PlayFabId,
                    Type = "master_player_account"
                },
                Statistics = new List<StatisticDelete> {
                    new (){
                        Name = "ScarecrowEasyScore",
                    },
                    new (){
                        Name = "ScarecrowMediumScore",
                    },
                    new (){
                        Name = "ScarecrowHardScore",
                    }
                }
            }, OnDeleteStatisticsSuccess, OnPlayFabError);
        }

        private void OnDeleteStatisticsSuccess(DeleteStatisticsResponse suc) {
            PlayFabAdminAPI.DeleteMasterPlayerAccount(new DeleteMasterPlayerAccountRequest {
                PlayFabId = PlayFabSettings.staticPlayer.PlayFabId,
            }, OnDeleteMasterPlayerAccountSuccess, OnPlayFabError);
        }

        private void OnPlayFabError(PlayFabError err) {
            Debug.LogError(err.GenerateErrorReport());
            deleteAccountErrorText.text = "There was an error deleting your account.";
        }
        private void OnDeleteMasterPlayerAccountSuccess(DeleteMasterPlayerAccountResult suc) {
            deleteAccountErrorText.text = "";
            PlayFabClientAPI.ForgetAllCredentials();
            PlayerAuthManager.DeleteAccount();
            PlayerDataManager.Instance.ResetPlayerData();
            GameStateManager.Instance.MoveToState(GameStateManager.GameState.Login);
        }

        public void OnCancelClicked() {
            deleteAccountConfirmationContainer.SetActive(false);
        }
        
        public void OnDeleteAccountAndDataClicked() {
            deleteAccountConfirmationContainer.SetActive(true);
        }
    }
}
