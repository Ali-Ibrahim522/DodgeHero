using UnityEngine;

namespace Events {
    public interface IEvent { }

    public struct HitEvent : IEvent {
        public int Gained;
    }
    public struct DeathEvent : IEvent {}
    public struct MissEventHealthUpdate : IEvent {
        public float DeathWait;
    }
    public struct MissEventStatsUpdate : IEvent {}
    
    public struct LevelSelectHeaderChangeEvent : IEvent {}
    
    public struct LevelSelectDiffChangeEvent : IEvent {
        public Sprite SelectedLevelPreview;
        public GameStateManager.GameState SelectedGameState;
    }

    public struct CrowPerchingEvent : IEvent {
        public int Channel;
        public float DestTime;
    }

    public struct CrowScaredEvent : IEvent {}

    public struct TargetHitEvent : IEvent {}
    
    public struct SpawnCrowEvent : IEvent {
        public float CrowSpeed;
        public GameObject CrowPrefab;
    }

    public struct RemoveCrowEvent : IEvent {
        public int Channel;
    }

    public struct LoadResultsDataEvent : IEvent {
        public Sprite ResultsSprite;
        public long Score;
        public int MaxCombo;
        public int TotalHits;
        public float Time;
    }

    public struct DeathEventStatsUpdate : IEvent {}
    
    public struct LoadLeaderboardEvent : IEvent {}
    
    public struct LoginWithScoreEvent : IEvent {}
    
    public struct EndChallengeSystemEvent : IEvent {}
}
