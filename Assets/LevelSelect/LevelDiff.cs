using System.Collections.Generic;
using Events;
using Global;
using UnityEngine;

namespace LevelSelect {
    public class LevelDiff : MonoBehaviour {
        [SerializeField] private Sprite levelPreview;
        [SerializeField] private GameStateManager.GameState levelDiffState;
        [SerializeField] private RectTransform levelPreviewRect;
        [SerializeField] private bool isDefault;
        private bool _selected;
        
        private EventProcessor<LevelSelectDiffChangeEvent> _onLevelSelectDiffChangeEventProcessor;

        private void Awake() => _onLevelSelectDiffChangeEventProcessor = new EventProcessor<LevelSelectDiffChangeEvent>(OnDiffDeselected);
        
        private void OnEnable() {
            _selected = false;
            if (isDefault) OnDiffSelected();
        }

        private void OnDisable() {
            if (_selected) OnDiffDeselected();
        }
        
        public void OnDiffSelected() {
            if (_selected) return;
            EventBus<LevelSelectDiffChangeEvent>.Publish(new LevelSelectDiffChangeEvent {
                SelectedLevelPreview = levelPreview,
                SelectedGameState = levelDiffState,
            });
            EventBus<LevelSelectDiffChangeEvent>.Subscribe(_onLevelSelectDiffChangeEventProcessor);
            Vector2 newButtonPos = levelPreviewRect.anchoredPosition;
            newButtonPos.x -= 45;
            levelPreviewRect.anchoredPosition = newButtonPos;
            _selected = true;
        }

        private void OnDiffDeselected() {
            _selected = false;
            EventBus<LevelSelectDiffChangeEvent>.Unsubscribe(_onLevelSelectDiffChangeEventProcessor);
            Vector2 newButtonPos = levelPreviewRect.anchoredPosition;
            newButtonPos.x += 45;
            levelPreviewRect.anchoredPosition = newButtonPos;
        }
    }
}
