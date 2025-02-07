using Global;
using Levels.Scarecrow;
using UnityEngine;

namespace Events {
    public interface IEvent { }
    public struct AttackChallengeInputEvent : IEvent {
        public int Channel;
    }
    public struct HitEvent : IEvent {
        public int Gained;
    }
    public struct DeathEvent : IEvent {}
    public struct MissEventHealthUpdate : IEvent {}
    public struct MissEventStatsUpdate : IEvent {}
    public struct LevelSelectHeaderChangeEvent : IEvent {}
    public struct LevelSelectDiffChangeEvent : IEvent {
        public Sprite SelectedLevelPreview;
        public GameStateManager.GameState SelectedGameState;
    }
    public struct CrowPerchingEvent : IEvent {
        public int Channel;
        public ElapsedContainer Progress;
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
    public struct PfpSelectedEvent : IEvent {
        public Texture SelectedTexture;
        public string FileName;
    }
    public struct LogChangesEvent : IEvent {}
    public struct AddChangeEvent : IEvent {
        public string ChangeLog;
    }
    public struct SaveSettingsEvent : IEvent {}
    public struct ResetSettingDefaultsEvent : IEvent {}
    public struct ReportLeaderboardSizeEvent : IEvent {
        public int LeaderboardSize;
    }
    public struct DisableSelectedSectionEvent : IEvent {}
    public struct LoadVisualSettingEvent : IEvent {}
    public struct LoadKeybindSettingEvent : IEvent {}
    public struct EnableKeybindModalEvent : IEvent {
        public string KeyName;
    }
    public struct DisableKeybindModalEvent : IEvent {}
}
