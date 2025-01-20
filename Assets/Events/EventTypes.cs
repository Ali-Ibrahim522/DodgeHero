using System;
using UnityEngine;

namespace Events {
    public interface IEvent { }

    public readonly struct HitEvent : IEvent {
        public int Gained { get; }
        public HitEvent(int gained) => Gained = gained;
    }
    public struct DeathEvent : IEvent {}
    public readonly struct MissEventHealthUpdate : IEvent {
        public float DeathWait { get; }
        public MissEventHealthUpdate(float deathWait) => DeathWait = deathWait;
    }
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

    public readonly struct CrowPerchingEvent : IEvent {
        public int Channel { get; }
        public Transform CrowPosition { get; }
        public Action OnCrowPerching { get; }
        public Vector3 Dest { get; }

        public CrowPerchingEvent(int channel, Transform crowPosition, Action onCrowPerching, Vector3 dest) {
            CrowPosition = crowPosition;
            OnCrowPerching = onCrowPerching;
            Dest = dest;
            Channel = channel;
        }
    }

    public readonly struct TargetHitEvent : IEvent {}
    
    public struct CrowDeathEvent : IEvent {}
    public class SpawnCrowEvent : IEvent {
        public float CrowSpeed { get; }
        public GameObject CrowPrefab { get; }
        public int CrowCountdown { get; set; }

        public SpawnCrowEvent(float crowSpeed, int crowCountdown, GameObject crowPrefab) {
            CrowSpeed = crowSpeed;
            CrowCountdown = crowCountdown;
            CrowPrefab = crowPrefab;
        }
    }

    public readonly struct CrowMissedEvent : IEvent {
        public int Channel { get; }

        public CrowMissedEvent(int channel) {
            Channel = channel;
        }
    }

}
