using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;

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
        Settings
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
    [SerializeField] private Texture2D defaultPfp;
    private Texture2D _pfp;
    private Dictionary<GameState, GameObject> _views;
    private GameState _lastLevelState;
    private readonly WaitForSeconds _wait = new (.001f);
    private PlayerProfileModel _playerDetails;
    private bool _playerIsGuest;
    private bool _deletedAccount;
    
    private void Awake() {
        if (Instance != null && Instance != this) Destroy(this); 
        else Instance = this;
        _deletedAccount = false;
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
    public void SetPlayerDetails(PlayerProfileModel playerDetails) {
        _playerDetails = playerDetails;
        StartCoroutine(SetPlayerPfp(playerDetails.AvatarUrl));
    }
    public IEnumerator SetPlayerPfp(string pfpName) {
        _playerDetails.AvatarUrl = pfpName;
        string filePath = $"{Application.streamingAssetsPath}/{pfpName}.jpg";
        
        #if !UNITY_WEBGL
            filePath = "file://" + filePath;
        #endif
        
        using UnityWebRequest uwr = UnityWebRequest.Get(filePath);
        yield return uwr.SendWebRequest();
        if (uwr.result == UnityWebRequest.Result.Success) {
            Texture2D texture = new (2, 2, TextureFormat.BGRA32,false);
            texture.LoadImage(uwr.downloadHandler.data);
            _pfp = texture;
        }
        else Debug.Log(uwr.error);
    }
    public void ResetPlayerDetails() {
        _playerIsGuest = false;
        _playerDetails = null;
    }
    public string GetDisplayName() => _playerIsGuest ? "Guest" : _playerDetails.DisplayName;
    public void SetDisplayName(string displayName) => _playerDetails.DisplayName = displayName;
    public string GetLastLevelName() => _lastLevelState.ToString();
    public Texture GetPfp() => _pfp;
    public string GetPfpFileName() => _playerDetails.AvatarUrl;
    public void SetPlayerAsGuest() {
        _playerIsGuest = true;
        _pfp = defaultPfp;
    }
    public void SetDeletedAccount(bool deleted) => _deletedAccount = deleted;
    public bool GetPlayerDeleted() => _deletedAccount;
    public void LogoutGuest() => _playerIsGuest = false;
    public bool IsPlayerGuest() => _playerIsGuest;
}
