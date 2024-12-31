using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Events {
    public static class EventBusFactory {
        public static IReadOnlyList<Type> EventTypes { get; set; } = new List<Type> {
            typeof(DeathEvent),
            typeof(HitEvent),
            typeof(MissEventHealthUpdate),
            typeof(MissEventStatsUpdate)
        };
        public static IReadOnlyList<Type> EventBusTypes { get; set; }
        
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

        public static void ClearAllBuses() {
            foreach (Type eventBusType in EventBusTypes) eventBusType.GetMethod("Clear", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, null);
        }
    }
}
