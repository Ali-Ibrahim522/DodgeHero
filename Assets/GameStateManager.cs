using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public enum GameState {
        Start,
        LevelSelect,
        ScarecrowEasy,
        ScarecrowMedium,
        ScarecrowHard
    }

    private GameState _currentState;

    [SerializeField] private List<GameObject> _views;
    private void Awake() {
        if (Instance != null && Instance != this) Destroy(this); 
        else Instance = this; 
    }
    void Start() {
        _views.ForEach(view => view.SetActive(false));
        _currentState = GameState.Start;
        _views[(int)_currentState].SetActive(true);
    }

    public void MoveToState(GameState nextState) {
        _views[(int)nextState].SetActive(true);
        _views[(int)_currentState].SetActive(false);
        _currentState = nextState;
    }
}
