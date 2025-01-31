using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events;
using PlayFab;
using PlayFab.ProgressionModels;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using EntityKey = PlayFab.ProgressionModels.EntityKey;
using GetPlayerProfileRequest = PlayFab.ClientModels.GetPlayerProfileRequest;
using PlayerProfileViewConstraints = PlayFab.ClientModels.PlayerProfileViewConstraints;
using StatisticUpdate = PlayFab.ProgressionModels.StatisticUpdate;

public class ResultView : MonoBehaviour {
    private EventProcessor<LoadResultsDataEvent> _onLoadResultsDataEvent;
    private EventProcessor<LoadLeaderboardEvent> _onLoadLeaderboardEvent;
    private EventProcessor<LoginWithScoreEvent> _onLoginWithScoreEvent;
    [SerializeField] private GameObject results;
    [SerializeField] private Image resultsBackground;
    [SerializeField] private TMP_Text maxComboText;
    [SerializeField] public TMP_Text scoreText;
    [SerializeField] public TMP_Text timeText;
    [SerializeField] public TMP_Text totalHitsText;
    [SerializeField] public List<ScoreComponents> leaderboardScoreComponents;
    [SerializeField] public ScoreComponents playerScoreComponents;
    [SerializeField] public GameObject guestHighscoreModal;
    [SerializeField] public GameObject loadingContainer;
    private string _leaderboardName;
    private List<String> _guestScores;
    private bool _top3;
    private string _playerName;
    void Awake() {
        _onLoadResultsDataEvent = new EventProcessor<LoadResultsDataEvent>(LoadResultsData);
        _onLoadLeaderboardEvent = new EventProcessor<LoadLeaderboardEvent>(CallGetLeaderboard);
        _onLoginWithScoreEvent = new EventProcessor<LoginWithScoreEvent>(LoginWithScore);
    }

    private void OnEnable() {
        EventBus<LoadResultsDataEvent>.Subscribe(_onLoadResultsDataEvent);
        EventBus<LoadLeaderboardEvent>.Subscribe(_onLoadLeaderboardEvent);
        EventBus<LoginWithScoreEvent>.Subscribe(_onLoginWithScoreEvent);
    }

    private void OnDisable() {
        EventBus<LoadResultsDataEvent>.Unsubscribe(_onLoadResultsDataEvent);
        EventBus<LoadLeaderboardEvent>.Unsubscribe(_onLoadLeaderboardEvent);
        EventBus<LoginWithScoreEvent>.Unsubscribe(_onLoginWithScoreEvent);
    }

    private void LoadResultsData(LoadResultsDataEvent loadResultsDataEventProps) {
        ClearLeaderboard();
        string strScore = loadResultsDataEventProps.Score.ToString();
        string strCombo = loadResultsDataEventProps.MaxCombo.ToString();
        string strTotalHits = loadResultsDataEventProps.TotalHits.ToString();
        string strTime = TimeSpan.FromSeconds(loadResultsDataEventProps.Time).ToString(@"mm\:ss\:fff");
        string lvlName = GameStateManager.Instance.GetLastLevelName();
        _leaderboardName = $"{lvlName}Leaderboard";
        
        resultsBackground.sprite = loadResultsDataEventProps.ResultsSprite;

        scoreText.text = strScore;
        maxComboText.text = $"{strCombo}x";
        totalHitsText.text = $"{strTotalHits}x";
        timeText.text = strTime;
        if (GameStateManager.Instance.IsPlayerGuest()) {
            guestHighscoreModal.SetActive(true);
            _guestScores = new List<string> {strScore, strCombo, strTotalHits, ((int)loadResultsDataEventProps.Time).ToString()};
        } else {
            PlayFabProgressionAPI.UpdateStatistics(new UpdateStatisticsRequest {
                Entity = new EntityKey {
                    Id = PlayFabSettings.staticPlayer.PlayFabId,
                    Type = "master_player_account"
                },
                Statistics = new List<StatisticUpdate> {
                    new() {
                        Name = $"{lvlName}Score",
                        Scores = new List<string> {
                            strScore,
                            strCombo,
                            strTotalHits,
                            ((int)loadResultsDataEventProps.Time).ToString(),
                        }
                    }
                }
            }, null, OnPlayFabError);
        }
    }

    private void LoginWithScore() {
        PlayFabProgressionAPI.UpdateStatistics(new UpdateStatisticsRequest {
            Statistics = new List<StatisticUpdate> {
                new() {
                    Name = $"{GameStateManager.Instance.GetLastLevelName()}Score",
                    Scores = new List<string> {
                        _guestScores[0],
                        _guestScores[1],
                        _guestScores[2],
                        _guestScores[3],
                    }
                }
            }
        }, null, OnPlayFabError);
    }

    private void CallGetLeaderboard() {
        guestHighscoreModal.SetActive(false);
        loadingContainer.SetActive(true);
        StartCoroutine(DelayedGetLeaderboard());
    }

    private IEnumerator DelayedGetLeaderboard() {
        yield return new WaitForSeconds(1f);
        PlayFabProgressionAPI.GetLeaderboard(new GetEntityLeaderboardRequest {
            LeaderboardName = _leaderboardName,
            StartingPosition = 1,
            PageSize = 3,
        }, OnGetLeaderboardSuccess, OnPlayFabError);
    }
    private void ClearLeaderboard() {
        loadingContainer.SetActive(false);
        foreach (ScoreComponents scoreComponents in leaderboardScoreComponents) {
            scoreComponents.pfp.texture = GameStateManager.Instance.GetDefaultPfp();
            scoreComponents.container.SetActive(false);
        }
        playerScoreComponents.pfp.texture = GameStateManager.Instance.GetDefaultPfp();
        playerScoreComponents.container.SetActive(false);
    }
    private void OnGetLeaderboardSuccess(GetEntityLeaderboardResponse suc) {
        _top3 = false;
        for (int i = 0; i < suc.Rankings.Count; i++) {
            if (suc.Rankings[i].Entity.Id.Equals(PlayFabSettings.staticPlayer.PlayFabId)) _top3 = true;
            SetScoreComponents(leaderboardScoreComponents[i], suc.Rankings[i]);
        }
        
        if (!_top3) {
            PlayFabProgressionAPI.GetLeaderboardAroundEntity(new GetLeaderboardAroundEntityRequest {
                LeaderboardName = _leaderboardName,
                MaxSurroundingEntries = 1
            }, (s) => { SetScoreComponents(playerScoreComponents, s.Rankings.Single(entry => entry.Entity.Id.Equals(PlayFabSettings.staticPlayer.PlayFabId)), true); }, OnPlayFabError);
        }
        loadingContainer.SetActive(false);
    }
    private void SetScoreComponents(ScoreComponents scoreComponents, EntityLeaderboardEntry entry, bool player = false) {
        if (player) {
            StartCoroutine(SetRankPfp(GameStateManager.Instance.GetAvatarUrl(), scoreComponents.pfp));
            scoreComponents.displayName.text = GameStateManager.Instance.GetDisplayName();
        }
        else SetScorePfp(scoreComponents.pfp, entry.Entity.Id, scoreComponents.displayName);
        scoreComponents.rank.text = $"{entry.Rank}.";
        scoreComponents.score.text = entry.Scores[0];
        scoreComponents.container.SetActive(true);
    }

    private void SetScorePfp(RawImage pfp, string playFabId, TMP_Text displayName) {
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
        if (pfpUrl == null) yield break;
        using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(pfpUrl);
        yield return uwr.SendWebRequest();
        if (uwr.result == UnityWebRequest.Result.Success) pfp.texture = DownloadHandlerTexture.GetContent(uwr);
        else Debug.Log(uwr.error);
    }
}
