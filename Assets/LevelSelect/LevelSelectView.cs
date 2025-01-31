using System.Collections;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace LevelSelect {
    public class LevelSelectView : MonoBehaviour {
        [SerializeField] private Image selectedLevelPreview;
        
        [Header("player info")]
        [SerializeField] private RawImage playerPfp;
        [SerializeField] private TMP_Text displayName;
        
        private GameStateManager.GameState _selectedGameState;
        private EventProcessor<LevelSelectDiffChangeEvent> _onLevelSelectChangeProcessor;
        
        public void Awake() => _onLevelSelectChangeProcessor = new EventProcessor<LevelSelectDiffChangeEvent>(SetSelectedLevel);
        public void OnEnable() {
            playerPfp.texture = GameStateManager.Instance.GetDefaultPfp();
            EventBus<LevelSelectDiffChangeEvent>.Subscribe(_onLevelSelectChangeProcessor);
            if (!GameStateManager.Instance.IsPlayerGuest()) StartCoroutine(SetPlayerPfp());
            displayName.text = GameStateManager.Instance.GetDisplayName();
        }
        public void OnDisable() => EventBus<LevelSelectDiffChangeEvent>.Unsubscribe(_onLevelSelectChangeProcessor);

        public void SetSelectedLevel(LevelSelectDiffChangeEvent levelSelectDiffChangeEventProps) {
            selectedLevelPreview.sprite = levelSelectDiffChangeEventProps.SelectedLevelPreview;
            _selectedGameState = levelSelectDiffChangeEventProps.SelectedGameState;
        }
        
        IEnumerator SetPlayerPfp() {
            string pfpUrl = GameStateManager.Instance.GetAvatarUrl();
            if (pfpUrl == null) yield break;
            using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(pfpUrl);
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success) playerPfp.texture = DownloadHandlerTexture.GetContent(uwr);
            else Debug.Log(uwr.error);
        }

        public void OnPlayButton() {
            if (_selectedGameState != GameStateManager.GameState.LevelSelect) {
                GameStateManager.Instance.StoreLastLevelState(_selectedGameState);
                GameStateManager.Instance.MoveToState(_selectedGameState);
            }
        }
    }
}
