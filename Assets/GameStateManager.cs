using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public enum GameState {
        Login,
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
    [SerializeField] private GameObject transitionObject;
    [SerializeField] private float transitionSpeed;
    [SerializeField] private Texture gloabalDefaultPfp;
    private Dictionary<GameState, GameObject> _views;
    private GameState _lastLevelState;
    private readonly WaitForSeconds _wait = new (.001f);
    private PlayerProfileModel _playerDetails;
    private bool _playerIsGuest;
    
    private void Awake() {
        if (Instance != null && Instance != this) Destroy(this); 
        else Instance = this;
        _playerIsGuest = false;
        transitionObject.SetActive(false);
        _views = new Dictionary<GameState, GameObject>();
        foreach (ViewPair pair in viewPairs) {
            pair.view.SetActive(false);
            _views[pair.state] = pair.view;
        }
        _currentState = GameState.Login;
        _views[_currentState].SetActive(true);
    }

    public void MoveToState(GameState nextState) => StartCoroutine(TransitionToState(nextState));

    private IEnumerator TransitionToState(GameState nextState) {
        transitionObject.SetActive(true);
        yield return StartCoroutine(FadeOut());
        _views[_currentState].SetActive(false);
        _views[nextState].SetActive(true);
        _currentState = nextState;
        yield return StartCoroutine(FadeIn());
        transitionObject.SetActive(false);
    }
    private IEnumerator FadeIn() {
        Color color = transitionImage.color;
        color.a = 1f;
        do {
            transitionImage.color = color;
            color.a -= .025f;
            yield return _wait;
        } while (color.a >= 0f);
        color.a = 0f;
        transitionImage.color = color;
    }
    
    private IEnumerator FadeOut() {
        Color color = transitionImage.color;
        color.a = 0f;
        do {
            transitionImage.color = color;
            color.a += .025f;
            yield return _wait;
        } while (color.a <= 1f);
        yield return new WaitForSeconds(.3f);
    }

    public void StoreLastLevelState(GameState levelState) => _lastLevelState = levelState;
    public void LoadLastLevel() => StartCoroutine(TransitionToState(_lastLevelState));
    public void SetPlayerDetails(PlayerProfileModel playerDetails) => _playerDetails = playerDetails;
    public void ResetPlayerDetails() {
        _playerIsGuest = false;
        _playerDetails = null;
    }
    public string GetAvatarUrl() => _playerDetails.AvatarUrl;
    public string GetDisplayName() => _playerIsGuest ? "Guest" : _playerDetails.DisplayName;
    public string GetLastLevelName() => _lastLevelState.ToString();
    public Texture GetDefaultPfp() => gloabalDefaultPfp;
    public void SetPlayerAsGuest() => _playerIsGuest = true;
    public void LogoutGuest() => _playerIsGuest = false;
    public bool IsPlayerGuest() => _playerIsGuest;
}
