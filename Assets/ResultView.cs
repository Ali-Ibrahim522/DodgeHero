using System;
using System.Globalization;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultView : MonoBehaviour {
    private EventProcessor<LoadResultsDataEvent> _onLoadResultsDataEvent;
    [SerializeField] private GameObject results;
    [SerializeField] private Image resultsBackground;
    [SerializeField] private TMP_Text maxComboText;
    [SerializeField] public TMP_Text scoreText;
    [SerializeField] public TMP_Text timeText;
    [SerializeField] public TMP_Text totalHitsText;

    void Awake() {
        _onLoadResultsDataEvent = new EventProcessor<LoadResultsDataEvent>(LoadResultsData);
    }

    private void OnEnable() {
        EventBus<LoadResultsDataEvent>.Subscribe(_onLoadResultsDataEvent);
    }

    private void OnDisable() {
        EventBus<LoadResultsDataEvent>.Unsubscribe(_onLoadResultsDataEvent);
    }

    private void LoadResultsData(LoadResultsDataEvent loadResultsDataEventProps) {
        resultsBackground.sprite = loadResultsDataEventProps.ResultsSprite;
        scoreText.text = loadResultsDataEventProps.Score.ToString();
        maxComboText.text = $"{loadResultsDataEventProps.MaxCombo}x";
        timeText.text = TimeSpan.FromSeconds(loadResultsDataEventProps.Time).ToString(@"mm\:ss\:fff");
        totalHitsText.text = $"{loadResultsDataEventProps.TotalHits}x";
    }
}
