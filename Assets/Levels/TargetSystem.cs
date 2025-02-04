using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Levels {
    public class TargetSystem : MonoBehaviour {
        public List<TargetChallenge> challenges;
        private float _elapsed;
        private float _window;
        private int _activeChallenge;
        private int _challengeCount;
        private int _maxWait;
        private EventProcessor<TargetHitEvent> _onTargetHitEventProcessor;
        private EventProcessor<DeathEventStatsUpdate> _onDeathEventProcessor;
        private Camera _camera;

        private enum TargetState {
            WaitingToPropose,
            Active,
            WaitingToFinish,
            Disabled
        }
        private TargetState _targetState;

        public void Awake() {
            _camera = Camera.main;
            _onTargetHitEventProcessor = new EventProcessor<TargetHitEvent>(OnTargetHit);
            _onDeathEventProcessor = new EventProcessor<DeathEventStatsUpdate>(DisableSystem);
        }
        
        private void DisableSystem() {
            StopAllCoroutines();
            _targetState = TargetState.Disabled;
        }

        void OnEnable() {
            _targetState = TargetState.WaitingToFinish;
            _challengeCount = 0;
            _elapsed = 0;
            _window = 0;
            _maxWait = 5;
            EventBus<DeathEventStatsUpdate>.Subscribe(_onDeathEventProcessor);
            EventBus<TargetHitEvent>.Subscribe(_onTargetHitEventProcessor);
        }

        void OnDisable() {
            EventBus<DeathEventStatsUpdate>.Unsubscribe(_onDeathEventProcessor);
            EventBus<TargetHitEvent>.Unsubscribe(_onTargetHitEventProcessor);
            foreach (TargetChallenge challenge in challenges) challenge.SetStump();
        }

        void Update() {
            switch (_targetState) {
                case TargetState.WaitingToPropose:
                    WaitingCheckForClick();
                    break;
                case TargetState.Active:
                    ActiveCheckForClick();
                    _elapsed += Time.deltaTime;
                    if (_elapsed >= _window && !challenges[_activeChallenge].done) SetTargetMissed();
                    break;
                case TargetState.WaitingToFinish:
                    WaitingCheckForClick();
                    _elapsed += Time.deltaTime;
                    if (_elapsed >= _window) {
                        challenges[_activeChallenge].SetStump();
                        StartCoroutine(StartNewTarget());
                    }
                    break;
            }
        }
        
        // y = 1/√(.01x + 1/9)
        void SetNewWindow() => _window = 1 / Mathf.Sqrt(.01f * _challengeCount + (1 / 9f));
        
        void WaitingCheckForClick() {
            if (Input.GetMouseButtonDown(0) && !Physics2D.GetRayIntersection(_camera.ScreenPointToRay(Input.mousePosition)).collider) {
                EventBus<MissEventStatsUpdate>.Publish(new MissEventStatsUpdate());
                EventBus<MissEventHealthUpdate>.Publish(new MissEventHealthUpdate());
            }
        }

        void ActiveCheckForClick() {
            if (Input.GetMouseButtonDown(0) && !Physics2D.GetRayIntersection(_camera.ScreenPointToRay(Input.mousePosition)).collider) {
               SetTargetMissed();
            }
        }

        IEnumerator StartNewTarget() {
            _targetState = TargetState.WaitingToPropose;
            yield return new WaitForSeconds(Random.Range(Math.Max(_maxWait - 3, 0), _maxWait));
            _activeChallenge = Random.Range(0, challenges.Count);
            challenges[_activeChallenge].SetTargetProposed();
            SetNewWindow();
            if (_challengeCount % 10 == 0 && _maxWait > 2) _maxWait--; 
            _challengeCount++;
            _elapsed = 0;
            _targetState = TargetState.Active;
        }

        private void SetTargetMissed() {
            challenges[_activeChallenge].SetTargetMissed();
            EventBus<MissEventStatsUpdate>.Publish(new MissEventStatsUpdate());
            EventBus<MissEventHealthUpdate>.Publish(new MissEventHealthUpdate());
            _elapsed = 0;
            _window *= .75f;
            _targetState = TargetState.WaitingToFinish;
            
        }

        private void OnTargetHit() {
            _targetState = TargetState.WaitingToFinish;
            int performed = (int)(100 * (1 + (1 - _elapsed / _window)));
            EventBus<HitEvent>.Publish(new HitEvent {
                Gained = performed
            });
            _elapsed = 0;
            _window *= .75f;
        }
        
    }
}
