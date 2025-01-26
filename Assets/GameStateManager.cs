using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public enum GameState {
        Start,
        LevelSelect,
        Results,
        ScarecrowEasy,
        ScarecrowMedium,
        ScarecrowHard,
    }
    
    [Serializable]
    public struct ViewPair {
        public GameState state;
        public GameObject view;
    }

    private GameState _currentState;

    [SerializeField] private List<ViewPair> viewPairs;
    [SerializeField] private Image transitionImage;
    [SerializeField] private float transitionSpeed;
    private Dictionary<GameState, GameObject> _views;
    private GameState _lastLevelState;
    
    private void Awake() {
        if (Instance != null && Instance != this) Destroy(this); 
        else Instance = this;
        _views = new Dictionary<GameState, GameObject>();
        foreach (ViewPair pair in viewPairs) {
            pair.view.SetActive(false);
            _views[pair.state] = pair.view;
        }
        _currentState = GameState.Start;
        _views[_currentState].SetActive(true);
    }

    public void MoveToState(GameState nextState) => StartCoroutine(TransitionToState(nextState));

    private IEnumerator TransitionToState(GameState nextState) {
        yield return StartCoroutine(FadeOut());
        _views[nextState].SetActive(true);
        _views[_currentState].SetActive(false);
        _currentState = nextState;
        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn() {
        Color color = transitionImage.color;
        color.a = 1f;
        do {
            transitionImage.color = color;
            color.a -= .01f;
            yield return new WaitForSeconds(.00001f);
        } while (color.a >= 0f);
        color.a = 0f;
        transitionImage.color = color;
    }
    
    private IEnumerator FadeOut() {
        Color color = transitionImage.color;
        color.a = 0f;
        do {
            transitionImage.color = color;
            color.a += .01f;
            yield return new WaitForSeconds(.00001f);
        } while (color.a <= 1f);
        yield return new WaitForSeconds(.2f);
    }

    public void StoreLastLevelState(GameState levelState) => _lastLevelState = levelState;
    
    public void LoadLastLevel() => StartCoroutine(TransitionToState(_lastLevelState));
}
