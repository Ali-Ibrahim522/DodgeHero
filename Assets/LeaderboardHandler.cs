using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.ProgressionModels;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using EntityKey = PlayFab.ProgressionModels.EntityKey;

public class LeaderboardHandler : MonoBehaviour {
    [SerializeField] private List<ScoreComponents> leaderboardScoreComponents;
    [SerializeField] private ScoreComponents playerScoreComponents;
    [SerializeField] private GameObject guestHighscoreModal;
    [SerializeField] private GameObject loadingContainer;
    [SerializeField] private bool delay;
    
    private string _leaderboardName;
    private int _completedEntries;
    private int _leaderboardSize;
    private bool _top;

    private void Awake() => ClearLeaderboard();
    private void OnEnable() {
        _leaderboardName = $"{GameStateManager.Instance.GetLastLevelName()}Leaderboard";
        CallGetLeaderboard();
    }

    private void OnDisable() {
        StopAllCoroutines();
        ClearLeaderboard();    
    }

    private void CallGetLeaderboard() {
        if (GameStateManager.Instance.IsPlayerGuest()) {
            guestHighscoreModal.SetActive(true);
        } else {
            guestHighscoreModal.SetActive(false);
            loadingContainer.SetActive(true);
            StartCoroutine(DelayedGetLeaderboard());
        }
    }

    private void ClearLeaderboard() {
        loadingContainer.SetActive(false);
        guestHighscoreModal.SetActive(false);
        foreach (ScoreComponents leaderboardScoreComponent in leaderboardScoreComponents) leaderboardScoreComponent.container.SetActive(false);
        playerScoreComponents.container.SetActive(false);
    }
    
    private IEnumerator DelayedGetLeaderboard() {
        yield return new WaitForSeconds(delay ? 1f : .5f);
        PlayFabProgressionAPI.GetLeaderboard(new GetEntityLeaderboardRequest {
            LeaderboardName = _leaderboardName,
            StartingPosition = 1,
            PageSize = (uint)leaderboardScoreComponents.Count,
        }, OnGetLeaderboardSuccess, OnPlayFabError);
    }

    private void OnGetLeaderboardSuccess(GetEntityLeaderboardResponse suc) {
        _top = false;
        _completedEntries = 0;
        _leaderboardSize = suc.Rankings.Count;
        for (int i = 0; i < suc.Rankings.Count; i++) {
            if (suc.Rankings[i].Entity.Id.Equals(PlayFabSettings.staticPlayer.PlayFabId)) _top = true;
            SetScoreComponents(leaderboardScoreComponents[i], suc.Rankings[i], false);
        }
        if (!_top) {
            PlayFabProgressionAPI.GetLeaderboardAroundEntity(new GetLeaderboardAroundEntityRequest {
                Entity = new EntityKey {
                    Id = PlayFabSettings.staticPlayer.PlayFabId,
                    Type = "master_player_account"
                },
                LeaderboardName = _leaderboardName,
                MaxSurroundingEntries = 1
            }, (s) => {
                EntityLeaderboardEntry playerEntry = s.Rankings.SingleOrDefault(entry => entry.Entity.Id.Equals(PlayFabSettings.staticPlayer.PlayFabId));
                if (playerEntry != null) {
                    SetScoreComponents(playerScoreComponents, playerEntry, true);
                }
            }, OnPlayFabError);
        }
        if (_leaderboardSize == 0) EndLoading();
    }
    private void SetScoreComponents(ScoreComponents scoreComponents, EntityLeaderboardEntry entry, bool player) {
        if (player) {
            scoreComponents.pfp.texture = GameStateManager.Instance.GetPfp();
            scoreComponents.displayName.text = GameStateManager.Instance.GetDisplayName();
        }
        else SetScoreInfo(scoreComponents.pfp, entry.Entity.Id, scoreComponents.displayName);
        scoreComponents.rank.text = $"{entry.Rank}.";
        scoreComponents.score.text = entry.Scores[0];
        scoreComponents.container.SetActive(true);
    }

    private void SetScoreInfo(RawImage pfp, string playFabId, TMP_Text displayName) {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints {
                ShowAvatarUrl = true,
                ShowDisplayName = true,
            }
        }, result => {
            displayName.text = result.PlayerProfile.DisplayName;
            StartCoroutine(SetRankPfp(result.PlayerProfile.AvatarUrl, pfp));
        }, OnPlayFabError);
    }
    private void OnPlayFabError(PlayFabError err) {
        Debug.Log(err.ApiEndpoint);
        Debug.Log(err.GenerateErrorReport());
    }
    private IEnumerator SetRankPfp(string pfpUrl, RawImage pfp) {
        string filePath = $"{Application.streamingAssetsPath}/{pfpUrl}.jpg";
    
        #if !UNITY_WEBGL
            filePath = "file://" + filePath;
        #endif
    
        using UnityWebRequest uwr = UnityWebRequest.Get(filePath);
        yield return uwr.SendWebRequest();
        if (uwr.result == UnityWebRequest.Result.Success) {
            Texture2D texture = new (2, 2, TextureFormat.BGRA32,false);
            texture.LoadImage(uwr.downloadHandler.data);
            pfp.texture = texture;
        }
        else Debug.Log(uwr.error);
        if (++_completedEntries == _leaderboardSize) EndLoading();
    }

    private void EndLoading() {
        EventBus<ReportLeaderboardSizeEvent>.Publish(new ReportLeaderboardSizeEvent {
            LeaderboardSize = _leaderboardSize
        });
        loadingContainer.SetActive(false);
    }
}