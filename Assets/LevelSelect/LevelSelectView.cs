using System;
using Events;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LevelSelect {
    public class LevelSelectView : MonoBehaviour {
        [SerializeField] private LevelHeader defaultLevel;
        private Image _selectedLevelPreview;
        private GameStateManager.GameState _selectedGameState = GameStateManager.GameState.Level;
        private EventProcessor<LevelSelectDiffChangeEvent> _onLevelSelectChangeProcessor;
        
        public void Awake() => _onLevelSelectChangeProcessor = new EventProcessor<LevelSelectDiffChangeEvent>(SetSelectedLevel);
        public void OnEnable() {
            EventBus<LevelSelectDiffChangeEvent>.Subscribe(_onLevelSelectChangeProcessor);
            defaultLevel.OnLevelHeaderClicked();
        }
        public void OnDisable()=> EventBus<LevelSelectDiffChangeEvent>.Unsubscribe(_onLevelSelectChangeProcessor);

        public void SetSelectedLevel(LevelSelectDiffChangeEvent levelSelectDiffChangeEventProps) {
            _selectedLevelPreview.sprite = levelSelectDiffChangeEventProps.SelectedLevelPreview;
            _selectedGameState = levelSelectDiffChangeEventProps.SelectedGameState;
        }
        
        public void OnPlayButton() => GameStateManager.Instance.MoveToState(_selectedGameState);
    }
}
