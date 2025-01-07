using System;
using Events;
using UnityEngine;

namespace LevelSelect {
    public class LevelDiff : MonoBehaviour {
        [SerializeField] private Sprite levelPreview;
        [SerializeField] private GameStateManager.GameState levelDiffState;
        [SerializeField] private GameObject levelDiffObject;
        [SerializeField] private bool selected;
        private EventProcessor<LevelSelectDiffChangeEvent> _onLevelSelectDiffChange;

        public void Awake() => _onLevelSelectDiffChange = new EventProcessor<LevelSelectDiffChangeEvent>(OnDiffDeselected);
        public void OnEnable() {
            if (selected) EventBus<LevelSelectDiffChangeEvent>.Publish(new LevelSelectDiffChangeEvent(levelPreview, levelDiffState));
        }
        
        public void OnDisable() => EventBus<LevelSelectDiffChangeEvent>.Unsubscribe(_onLevelSelectDiffChange);
        
        public void OnDiffSelected() {
            EventBus<LevelSelectDiffChangeEvent>.Publish(new LevelSelectDiffChangeEvent(levelPreview, levelDiffState));
            EventBus<LevelSelectDiffChangeEvent>.Subscribe(_onLevelSelectDiffChange);
            Vector3 newButtonPos = levelDiffObject.transform.position;
            newButtonPos.x -= 45;
            levelDiffObject.transform.position = newButtonPos;
        }

        public void OnDiffDeselected() {
            EventBus<LevelSelectDiffChangeEvent>.Unsubscribe(_onLevelSelectDiffChange);
            Vector3 newButtonPos = levelDiffObject.transform.position;
            newButtonPos.x += 45;
            levelDiffObject.transform.position = newButtonPos;
        }
        
        public void DisplayLevelDiff() => levelDiffObject.SetActive(true);
    }
}
