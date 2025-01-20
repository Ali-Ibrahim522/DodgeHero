using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

namespace Levels
{
    public class AttackSystem : MonoBehaviour {
        private int _challengeCount;
        private int _activeChallenge;
        private float _elapsed;
        private float _window;
        private bool _complete;
        public SpriteRenderer attack;
        public List<AttackChallenge> challenges;
        void OnEnable() {
            _challengeCount = 0;
            SetChallengeProposed();
        }

        void OnDisable() => ResetChallenge();
    
        void Update() {
            _elapsed += Time.deltaTime;
            if (_complete) WaitForWindow();
            else CheckingForInput();
        }
        
        // y = 1/√(.01x + 1/3)
        void SetNewWindow() => _window = 1 / Mathf.Sqrt(.01f * _challengeCount + (1 / 3f));

        private void WaitForWindow() {
            if (_elapsed >= _window) {
                ResetChallenge();
                SetChallengeProposed();
            }
        }

        private void CheckingForInput() {
            if (_elapsed < _window) {
                bool good = false;
                for (int i = 0; i < challenges.Count; i++) {
                    if (challenges[i].IsChallengePressed()) {
                        if (_activeChallenge == i) {
                            good = true;
                        } else {
                            SetChallengeMissed(false);
                            return;
                        }
                    }
                }
                if (good) SetChallengeHit();
            } else {
                SetChallengeMissed(true);
            }
        }
        

        void ResetChallenge() {
            attack.sprite = null;
            challenges[_activeChallenge].ResetChallenge();
        }

        void SetChallengeProposed() {
            _activeChallenge = Random.Range(0, challenges.Count);
            challenges[_activeChallenge].SetChallengeProposed(attack);
            SetNewWindow();
            _complete = false;
            _challengeCount++;
            _elapsed = 0;
        }

        void SetChallengeHit() {
            int performed = (int)(100 * (1 + (1 - _elapsed / _window)));
            challenges[_activeChallenge].SetChallengeHit(attack);
            _complete = true;
            EventBus<HitEvent>.Publish(new HitEvent(performed));
        }

        void SetChallengeMissed(bool ranOutOfTime) {
            _complete = true;
            if (ranOutOfTime) _window += .75f;
            challenges[_activeChallenge].SetChallengeMissed(attack);
            EventBus<MissEventStatsUpdate>.Publish(new MissEventStatsUpdate());
            EventBus<MissEventHealthUpdate>.Publish(new MissEventHealthUpdate(_window - _elapsed));
        }
    }
}
