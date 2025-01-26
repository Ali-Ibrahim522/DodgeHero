using Events;
using UnityEngine;

namespace LevelSelect {
    public class LevelDiff : MonoBehaviour {
        [SerializeField] private Sprite levelPreview;
        [SerializeField] private GameStateManager.GameState levelDiffState;
        [SerializeField] private RectTransform levelPreviewRect;
        [SerializeField] private bool isDefault;
        private bool _selected;
        private EventProcessor<LevelSelectDiffChangeEvent> _onLevelSelectDiffChange;

        public void Awake() => _onLevelSelectDiffChange = new EventProcessor<LevelSelectDiffChangeEvent>(OnDiffDeselected);
        public void OnEnable() {
            _selected = false;
            if (isDefault) OnDiffSelected();
        }

        public void OnDisable() {
            if (_selected) OnDiffDeselected();
        }
        
        public void OnDiffSelected() {
            if (_selected) return;
            EventBus<LevelSelectDiffChangeEvent>.Publish(new LevelSelectDiffChangeEvent {
                SelectedLevelPreview = levelPreview,
                SelectedGameState = levelDiffState
            });
            EventBus<LevelSelectDiffChangeEvent>.Subscribe(_onLevelSelectDiffChange);
            Vector2 newButtonPos = levelPreviewRect.anchoredPosition;
            newButtonPos.x -= 45;
            levelPreviewRect.anchoredPosition = newButtonPos;
            _selected = true;
        }

        public void OnDiffDeselected() {
            _selected = false;
            EventBus<LevelSelectDiffChangeEvent>.Unsubscribe(_onLevelSelectDiffChange);
            Vector2 newButtonPos = levelPreviewRect.anchoredPosition;
            newButtonPos.x += 45;
            levelPreviewRect.anchoredPosition = newButtonPos;
        }
    }
}
