﻿using Events;
using UnityEngine;

namespace LevelSelect {
    public class LevelHeader : MonoBehaviour {
        [SerializeField] private GameObject levelDiffs;
        [SerializeField] private bool isDefault;
        private bool _selected;
        
        private EventProcessor<LevelSelectHeaderChangeEvent> _onLevelSelectHeaderChangeEventProcessor;

        private void Awake() => _onLevelSelectHeaderChangeEventProcessor = new EventProcessor<LevelSelectHeaderChangeEvent>(DisableLevelDiffs);
        
        public void Start() {
            _selected = false;
            if (isDefault) OnLevelHeaderClicked();
        }

        public void OnDisable() {
            if (_selected) DisableLevelDiffs();
        }

        public void OnLevelHeaderClicked() {
            if (_selected) return;
            EventBus<LevelSelectHeaderChangeEvent>.Publish(new LevelSelectHeaderChangeEvent());
            EventBus<LevelSelectHeaderChangeEvent>.Subscribe(_onLevelSelectHeaderChangeEventProcessor);
            levelDiffs.SetActive(true);
            _selected = true;
        }

        private void DisableLevelDiffs() {
            EventBus<LevelSelectHeaderChangeEvent>.Unsubscribe(_onLevelSelectHeaderChangeEventProcessor);
            levelDiffs.SetActive(false);
            _selected = false;
        }
    }
}
