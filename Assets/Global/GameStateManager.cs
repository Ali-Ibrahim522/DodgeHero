using System;
using System.Collections;
using System.Collections.Generic;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Global {
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
            public SceneReference view;
        }

        private GameState _currentState;

        [SerializeField] private List<ViewPair> viewPairs;
        [SerializeField] private Image transitionImage;
        [SerializeField] private GameObject transitionObject;
        private Dictionary<GameState, SceneReference> _views;
        private GameState _lastLevelState;
        private readonly WaitForSeconds _wait = new (.001f);
    
        private void Awake() {
            if (Instance != null && Instance != this) Destroy(this); 
            else Instance = this;
            transitionObject.SetActive(false);
            _views = new Dictionary<GameState, SceneReference>();
            foreach (ViewPair pair in viewPairs) {
                _views[pair.state] = pair.view;
            }
            _currentState = GameState.Login;
        }

        private void Start() => SceneManager.LoadSceneAsync(_views[_currentState].Path, LoadSceneMode.Additive);

        public void MoveToState(GameState nextState) => StartCoroutine(TransitionToState(nextState));

        private IEnumerator TransitionToState(GameState nextState) {
            transitionObject.SetActive(true);
            yield return StartCoroutine(FadeOut());
            AsyncOperation unload = SceneManager.UnloadSceneAsync(_views[_currentState].Path);
            AsyncOperation load = SceneManager.LoadSceneAsync(_views[nextState].Path, LoadSceneMode.Additive);
            _currentState = nextState;
            yield return new WaitWhile(() => !unload.isDone || !load.isDone);
            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(_views[nextState].Path));
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
        public string GetLastLevelName() => _lastLevelState.ToString();
    }
}
