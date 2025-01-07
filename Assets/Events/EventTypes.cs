using UnityEngine;

namespace Events {
    public interface IEvent { }

    public readonly struct HitEvent : IEvent {
        public int Gained { get; }
        public HitEvent(int gained) => Gained = gained;
    }
    public struct DeathEvent : IEvent {}
    public struct MissEventHealthUpdate : IEvent {}
    public struct MissEventStatsUpdate : IEvent {}
    
    public struct LevelSelectHeaderChangeEvent : IEvent {}
    
    
    public readonly struct LevelSelectDiffChangeEvent : IEvent {
        public Sprite SelectedLevelPreview { get; }
        public GameStateManager.GameState SelectedGameState { get; }
        public LevelSelectDiffChangeEvent(Sprite selectedLevelPreview, GameStateManager.GameState selectedGameState) {
            SelectedLevelPreview = selectedLevelPreview;
            SelectedGameState = selectedGameState;
        }
    }

    public readonly struct TargetHitEvent : IEvent {}
}
