using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Levels {
    public class TargetSystem : MonoBehaviour {
        public List<TargetChallenge> _challenges;
        private float _elapsed;
        private float _window;
        private int _activeChallenge;
        private int _challengeCount;
        private bool _waiting;
        private int _maxWait;
        private EventProcessor<TargetHitEvent> _onTargetHitEventProcessor;

        public void Awake() => _onTargetHitEventProcessor = new EventProcessor<TargetHitEvent>(OnTargetHit);
        
        void OnEnable() {
            _challengeCount = 0;
            _elapsed = 0;
            _window = 0;
            _waiting = false;
            _maxWait = 5;
            EventBus<TargetHitEvent>.Subscribe(_onTargetHitEventProcessor);
        }

        void OnDisable() {
            EventBus<TargetHitEvent>.Unsubscribe(_onTargetHitEventProcessor);
            foreach (TargetChallenge challenge in _challenges) challenge.SetStump();
        }
        void Update() {
            if (_waiting) return;
            if (_elapsed >= _window) {
                _challenges[_activeChallenge].SetStump();
                StartCoroutine(StartNewTarget());
            } else {
                _elapsed += Time.deltaTime;
                if (_elapsed >= _window && !_challenges[_activeChallenge].done) StartCoroutine(SetTargetMissed());
            }
        }
        
        // y = 1/√(.01x + 1/9)
        void SetNewWindow() => _window = 1 / Mathf.Sqrt(.01f * _challengeCount + (1 / 9f));

        IEnumerator StartNewTarget() {
            _waiting = true;
            yield return new WaitForSeconds(Random.Range(Math.Max(_maxWait - 3, 0), _maxWait));
            _activeChallenge = Random.Range(0, _challenges.Count);
            _challenges[_activeChallenge].SetTargetProposed();
            SetNewWindow();
            if (_challengeCount % 10 == 0 && _maxWait > 2) _maxWait--; 
            _challengeCount++;
            _elapsed = 0;
            _waiting = false;
        }

        IEnumerator SetTargetMissed() {
            _waiting = true;
            _challenges[_activeChallenge].SetTargetMissed();
            yield return new WaitForSeconds(.75f);
            EventBus<MissEventHealthUpdate>.Publish(new MissEventHealthUpdate());
            EventBus<MissEventStatsUpdate>.Publish(new MissEventStatsUpdate());
            _waiting = false;
        }

        void OnTargetHit() {
            int performed = (int)(100 * (1 + (1 - _elapsed / _window)));
            EventBus<HitEvent>.Publish(new HitEvent(performed));
        }
        
    }
}
