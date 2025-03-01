using System;
using System.Collections.Generic;
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

        private void OnEnable() => LoadResultsData();

        private void LoadResultsData() {
            SessionScore newScore = SessionScoreManager.LoadScore();
            string strScore = newScore.Score.ToString();
            string strCombo = newScore.MaxCombo.ToString();
            string strTotalHits = newScore.TotalHits.ToString();
            string strTime = TimeSpan.FromSeconds(newScore.Time).ToString(@"mm\:ss\:fff");
            string lvlName = GameStateManager.Instance.GetLastLevelName();
        
            resultsBackground.sprite = newScore.ResultsSprite;
            scoreText.text = strScore;
            maxComboText.text = $"{strCombo}x";
            totalHitsText.text = $"{strTotalHits}x";
            timeText.text = strTime;
            
            if (PlayerAuthManager.GetPlayerState() != PlayerAuthManager.PlayerState.Guest) {
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
                                ((int)newScore.Time).ToString(),
                            }
                        }
                    }
                }, null, OnPlayFabError);
            }
        }
    
        private void OnPlayFabError(PlayFabError err) {
            Debug.Log(err.ApiEndpoint);
            Debug.Log(err.GenerateErrorReport());
        }
    }
}
