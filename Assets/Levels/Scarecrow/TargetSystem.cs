using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using Global;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Levels.Scarecrow {
    public class TargetSystem : MonoBehaviour {
        [SerializeField] private InputActionReference hitTargetKey;
        [SerializeField] private Transform clickHitFadeScale;
        public List<TargetChallenge> challenges;
        private float _elapsed;
        private float _window;
        private int _activeChallenge;
        private int _challengeCount;
        private int _maxWait;
        private Camera _camera;
        private Color _proposedColor;
        private Color _hitColor;
        private Color _missedColor;

        private EventProcessor<DeathEventStatsUpdate> _onDeathEventStatsUpdateProcessor;

        private enum TargetState {
            WaitingToPropose,
            Active,
            WaitingToFinish,
            Disabled
        }
        private TargetState _targetState;

        public void Awake() {
            _camera = Camera.main;
            _onDeathEventStatsUpdateProcessor = new EventProcessor<DeathEventStatsUpdate>(DisableSystem);
        }
        
        private void DisableSystem() {
            StopAllCoroutines();
            _targetState = TargetState.Disabled;
        }

        void OnEnable() {
            _proposedColor = PlayerDataManager.Instance.visuals.proposed;
            _hitColor = PlayerDataManager.Instance.visuals.hit;
            _missedColor = PlayerDataManager.Instance.visuals.missed;
            _targetState = TargetState.WaitingToFinish;
            _challengeCount = 0;
            _elapsed = 0;
            _window = 0;
            _maxWait = 5;
            hitTargetKey.action.started += ActiveCheckForClick;
            EventBus<DeathEventStatsUpdate>.Subscribe(_onDeathEventStatsUpdateProcessor);
        }

        void OnDisable() {
            hitTargetKey.action.started -= ActiveCheckForClick;
            EventBus<DeathEventStatsUpdate>.Unsubscribe(_onDeathEventStatsUpdateProcessor);
            foreach (TargetChallenge challenge in challenges) challenge.SetStump();
        }
        
        private void ActiveCheckForClick(InputAction.CallbackContext obj) {
            Vector3 pos = _camera.ScreenToWorldPoint(Input.mousePosition);
            pos.z = clickHitFadeScale.position.z;
            clickHitFadeScale.position = pos;
            Collider2D hit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(Input.mousePosition)).collider;
            if (hit) {
                if (challenges[_activeChallenge].OnTargetHit()) {
                    challenges[_activeChallenge].SetTargetHit(_hitColor);
                    OnTargetHit();
                }
            } else {
                EventBus<MissEventStatsUpdate>.Publish(new MissEventStatsUpdate());
                EventBus<MissEventHealthUpdate>.Publish(new MissEventHealthUpdate());
            }
        }

        void Update() {
            switch (_targetState) {
                case TargetState.Active:
                    _elapsed += Time.deltaTime;
                    if (_elapsed >= _window && !challenges[_activeChallenge].done) SetTargetMissed();
                    break;
                case TargetState.WaitingToFinish:
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

        IEnumerator StartNewTarget() {
            _targetState = TargetState.WaitingToPropose;
            yield return new WaitForSeconds(Random.Range(Math.Max(_maxWait - 3, 0), _maxWait));
            _activeChallenge = Random.Range(0, challenges.Count);
            challenges[_activeChallenge].SetTargetProposed(_proposedColor);
            SetNewWindow();
            if (_challengeCount % 10 == 0 && _maxWait > 2) _maxWait--; 
            _challengeCount++;
            _elapsed = 0;
            _targetState = TargetState.Active;
        }

        private void SetTargetMissed() {
            challenges[_activeChallenge].SetTargetMissed(_missedColor);
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
