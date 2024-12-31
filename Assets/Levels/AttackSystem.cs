using System.Collections.Generic;
using Events;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.UI;

namespace Levels
{
    public class AttackSystem : MonoBehaviour {
        private int _challengeCount;
        private int _activeChallenge;
        private float _elapsed;
        private float _window;
        private bool _complete;
        public Image attack;
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
        
        // y = 1/√(.01x + 1/9)
        void SetNewWindow() {
            _window = 1 / Mathf.Sqrt(.01f * _challengeCount + (1 / 3f));
        }

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
                    if (Input.GetKeyDown(challenges[i].key)) {
                        if (_activeChallenge == i) {
                            good = true;
                        } else {
                            SetChallengeMissed();
                            return;
                        }
                    }
                }
                if (good) SetChallengeHit();
            } else {
                _window += .5f;
                SetChallengeMissed();
            }
        }
        

        void ResetChallenge() {
            attack.sprite = null;
            challenges[_activeChallenge].arrow.color = Color.white;
        }

        void SetChallengeProposed() {
            _activeChallenge = Random.Range(0, challenges.Count);
            attack.sprite = challenges[_activeChallenge].pre;
            challenges[_activeChallenge].arrow.color = Color.cyan;
            _complete = false;
            SetNewWindow();
            _challengeCount++;
            _elapsed = 0;
        }

        void SetChallengeHit() {
            int performed = (int)(1000 * (1 + (1 - _elapsed / _window)));
            attack.sprite = challenges[_activeChallenge].post;
            challenges[_activeChallenge].arrow.color = Color.green;
            _complete = true;
            EventBus<HitEvent>.Publish(new HitEvent(performed));
        }

        void SetChallengeMissed() {
            attack.sprite = challenges[_activeChallenge].post;
            challenges[_activeChallenge].arrow.color = Color.red;
            _complete = true;
            EventBus<MissEventHealthUpdate>.Publish(new MissEventHealthUpdate());
            EventBus<MissEventStatsUpdate>.Publish(new MissEventStatsUpdate());
        }
    }
}
