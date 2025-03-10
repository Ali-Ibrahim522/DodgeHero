﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Events {
    public static class EventBusFactory {
        private static IReadOnlyList<Type> EventTypes { get; set; } = new List<Type> {
            typeof(DeathEvent),
            typeof(HitEvent),
            typeof(MissEventHealthUpdate),
            typeof(MissEventStatsUpdate),
            typeof(LevelSelectDiffChangeEvent),
            typeof(LevelSelectHeaderChangeEvent),
            typeof(TargetHitEvent),
            typeof(SpawnCrowEvent),
            typeof(CrowPerchingEvent),
            typeof(RemoveCrowEvent),
            typeof(DeathEventStatsUpdate),
            typeof(CrowScaredEvent),
            typeof(PfpSelectedEvent),
            typeof(LogChangesEvent),
            typeof(AddChangeEvent),
            typeof(SaveSettingsEvent),
            typeof(ResetSettingDefaultsEvent),
            typeof(ReportLeaderboardSizeEvent),
            typeof(AttackChallengeInputEvent),
            typeof(DisableSelectedSectionEvent),
            typeof(LoadVisualSettingEvent),
            typeof(LoadKeybindSettingEvent),
            typeof(EnableKeybindModalEvent),
            typeof(DisableKeybindModalEvent),
            typeof(ReleaseAudioPlayerEvent),
            typeof(PlayAudioEvent),
            typeof(PlayMusicEvent),
            typeof(LoadAuditorySettingEvent)
        };

        private static IReadOnlyList<Type> EventBusTypes { get; set; }
        
        #if UNITY_EDITOR
            public static PlayModeStateChange PlayModeState { get; set; }

            [InitializeOnLoadMethod]
            public static void InitializeEditor() {
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            }

            static void OnPlayModeStateChanged(PlayModeStateChange state) {
                PlayModeState = state;
                if (state == PlayModeStateChange.ExitingEditMode) ClearAllBuses();
            }
        #endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize() => EventBusTypes = EventTypes.Select(eventType => typeof(EventBus<>).MakeGenericType(eventType)).ToList();

        private static void ClearAllBuses() {
            foreach (Type eventBusType in EventBusTypes) eventBusType.GetMethod("Clear", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, null);
        }
    }
}
