using System;
using System.Collections.Generic;
using Events;
using Global;
using PlayFab;
using PlayFab.ProgressionModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EntityKey = PlayFab.ProgressionModels.EntityKey;
using StatisticUpdate = PlayFab.ProgressionModels.StatisticUpdate;

namespace Results {
    public class ResultView : MonoBehaviour {
        [SerializeField] private Image resultsBackground;
        [SerializeField] private TMP_Text maxComboText;
        [SerializeField] public TMP_Text scoreText;
        [SerializeField] public TMP_Text timeText;
        [SerializeField] public TMP_Text totalHitsText;
        private List<string> _guestScores;
        
        private EventProcessor<LoadResultsDataEvent> _onLoadResultsDataEventProcessor;
        private EventProcessor<LoginWithScoreEvent> _onLoginWithScoreEventProcessor;

        private void Awake() {
            _onLoadResultsDataEventProcessor = new EventProcessor<LoadResultsDataEvent>(LoadResultsData);
            _onLoginWithScoreEventProcessor = new EventProcessor<LoginWithScoreEvent>(LoginWithScore);
        }
        
        private void OnEnable() {
            EventBus<LoadResultsDataEvent>.Subscribe(_onLoadResultsDataEventProcessor);
            EventBus<LoginWithScoreEvent>.Subscribe(_onLoginWithScoreEventProcessor);
        }

        private void OnDisable() {
            EventBus<LoadResultsDataEvent>.Unsubscribe(_onLoadResultsDataEventProcessor);
            EventBus<LoginWithScoreEvent>.Unsubscribe(_onLoginWithScoreEventProcessor);
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
            if (PlayerAuthManager.GetPlayerState() == PlayerAuthManager.PlayerState.Guest) {
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
}
