using System.Collections.Generic;
using Events;
using UnityEngine;

namespace LevelSelect {
    public class LevelHeader : MonoBehaviour {
        [SerializeField] private List<LevelDiff> levelDiffs;
        private EventProcessor<LevelSelectHeaderChangeEvent> _onLevelSelectHeaderChangeEventProcessor;

        public void Awake() => _onLevelSelectHeaderChangeEventProcessor = new EventProcessor<LevelSelectHeaderChangeEvent>(DisableLevelDiffs);
        
        public void OnLevelHeaderClicked() {
            EventBus<LevelSelectHeaderChangeEvent>.Publish(new LevelSelectHeaderChangeEvent());
            EventBus<LevelSelectHeaderChangeEvent>.Subscribe(_onLevelSelectHeaderChangeEventProcessor);
            foreach (LevelDiff levelDiff in levelDiffs) levelDiff.DisplayLevelDiff();
            levelDiffs[0].OnDiffSelected();
        }
        
        public void DisableLevelDiffs() {
            EventBus<LevelSelectHeaderChangeEvent>.Unsubscribe(_onLevelSelectHeaderChangeEventProcessor);
            foreach (LevelDiff levelDiff in levelDiffs) levelDiff.DisplayLevelDiff();
        }
    }
}
