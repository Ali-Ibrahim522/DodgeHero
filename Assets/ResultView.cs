using System;
using System.Collections.Generic;
using Events;
using PlayFab;
using PlayFab.ProgressionModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EntityKey = PlayFab.ProgressionModels.EntityKey;
using StatisticUpdate = PlayFab.ProgressionModels.StatisticUpdate;

public class ResultView : MonoBehaviour {
    private EventProcessor<LoadResultsDataEvent> _onLoadResultsDataEvent;
    private EventProcessor<LoginWithScoreEvent> _onLoginWithScoreEvent;
    [SerializeField] private Image resultsBackground;
    [SerializeField] private TMP_Text maxComboText;
    [SerializeField] public TMP_Text scoreText;
    [SerializeField] public TMP_Text timeText;
    [SerializeField] public TMP_Text totalHitsText;
    private List<string> _guestScores;

    void Awake() {
        _onLoadResultsDataEvent = new EventProcessor<LoadResultsDataEvent>(LoadResultsData);
        _onLoginWithScoreEvent = new EventProcessor<LoginWithScoreEvent>(LoginWithScore);
    }

    private void OnEnable() {
        EventBus<LoadResultsDataEvent>.Subscribe(_onLoadResultsDataEvent);
        EventBus<LoginWithScoreEvent>.Subscribe(_onLoginWithScoreEvent);
    }

    private void OnDisable() {
        EventBus<LoadResultsDataEvent>.Unsubscribe(_onLoadResultsDataEvent);
        EventBus<LoginWithScoreEvent>.Unsubscribe(_onLoginWithScoreEvent);
    }

    private void LoadResultsData(LoadResultsDataEvent loadResultsDataEventProps) {
        string strScore = loadResultsDataEventProps.Score.ToString();
        string strCombo = loadResultsDataEventProps.MaxCombo.ToString();
        string strTotalHits = loadResultsDataEventProps.TotalHits.ToString();
        string strTime = TimeSpan.FromSeconds(loadResultsDataEventProps.Time).ToString(@"mm\:ss\:fff");
        string lvlName = GameStateManager.Instance.GetLastLevelName();
        
        resultsBackground.sprite = loadResultsDataEventProps.ResultsSprite;

        scoreText.text = strScore;
        maxComboText.text = $"{strCombo}x";
        totalHitsText.text = $"{strTotalHits}x";
        timeText.text = strTime;
        if (GameStateManager.Instance.IsPlayerGuest()) {
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
            Entity = new EntityKey {
                Id = PlayFabSettings.staticPlayer.PlayFabId,
                Type = "master_player_account"
            },
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
    
    private void OnPlayFabError(PlayFabError err) {
        Debug.Log(err.ApiEndpoint);
        Debug.Log(err.GenerateErrorReport());
    }
}
