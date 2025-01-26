using Events;
using UnityEngine;
using UnityEngine.UI;

namespace LevelSelect {
    public class LevelSelectView : MonoBehaviour {
        [SerializeField] private Image selectedLevelPreview;
        private GameStateManager.GameState _selectedGameState;
        private EventProcessor<LevelSelectDiffChangeEvent> _onLevelSelectChangeProcessor;
        
        public void Awake() => _onLevelSelectChangeProcessor = new EventProcessor<LevelSelectDiffChangeEvent>(SetSelectedLevel);
        public void OnEnable() => EventBus<LevelSelectDiffChangeEvent>.Subscribe(_onLevelSelectChangeProcessor);
        public void OnDisable()=> EventBus<LevelSelectDiffChangeEvent>.Unsubscribe(_onLevelSelectChangeProcessor);

        public void SetSelectedLevel(LevelSelectDiffChangeEvent levelSelectDiffChangeEventProps) {
            selectedLevelPreview.sprite = levelSelectDiffChangeEventProps.SelectedLevelPreview;
            _selectedGameState = levelSelectDiffChangeEventProps.SelectedGameState;
        }

        public void OnPlayButton() {
            if (_selectedGameState != GameStateManager.GameState.LevelSelect) {
                GameStateManager.Instance.StoreLastLevelState(_selectedGameState);
                GameStateManager.Instance.MoveToState(_selectedGameState);
            }
        }
    }
}
