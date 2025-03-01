using System.Collections.Generic;
using Audio;
using Events;
using Global;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Levels.Scarecrow
{
    public class AttackSystem : MonoBehaviour {
        private int _challengeCount;
        private int _activeChallenge;
        private float _elapsed;
        private float _window;
        private bool _complete;
        [SerializeField] private SpriteRenderer attack;
        [SerializeField] private List<AttackChallenge> challenges;
        [SerializeField] private SoundEffectData attackSound;
        private Color _proposedColor;
        private Color _hitColor;
        private Color _missedColor;
        private EventProcessor<AttackChallengeInputEvent> _onAttackChallengeInputEventProcessor;
        private EventProcessor<DeathEventStatsUpdate> _onDeathEventStatsUpdateEventProcessor;

        private void Awake() {
            _onAttackChallengeInputEventProcessor = new EventProcessor<AttackChallengeInputEvent>(AttackKeyHit);
            _onDeathEventStatsUpdateEventProcessor = new EventProcessor<DeathEventStatsUpdate>(DisableSystem);
        }
        
        void OnEnable() {
            _proposedColor = PlayerDataManager.Instance.visuals.proposed;
            _hitColor = PlayerDataManager.Instance.visuals.hit;
            _missedColor = PlayerDataManager.Instance.visuals.missed;
            EventBus<AttackChallengeInputEvent>.Subscribe(_onAttackChallengeInputEventProcessor);
            EventBus<DeathEventStatsUpdate>.Subscribe(_onDeathEventStatsUpdateEventProcessor);
            foreach (AttackChallenge challenge in challenges) challenge.EnableAttackChallenge();
            _challengeCount = 0;
            attackSound.clip.LoadAudioData();
            SetChallengeProposed();
        }

        void OnDisable() {
            foreach (AttackChallenge challenge in challenges) challenge.DisableAttackChallenge();
            EventBus<AttackChallengeInputEvent>.Unsubscribe(_onAttackChallengeInputEventProcessor);
            EventBus<DeathEventStatsUpdate>.Unsubscribe(_onDeathEventStatsUpdateEventProcessor);
        }

        void Update() {
            _elapsed += Time.deltaTime;
            if (_complete) WaitForWindow();
            else CheckingForInput();
        }

        private void DisableSystem() {
            _complete = true;
            _window += 5f;
            attackSound.clip.UnloadAudioData();
        }

        void SetNewWindow() => _window = 1 / Mathf.Sqrt(.01f * _challengeCount + (1 / 3f));

        private void WaitForWindow() {
            if (_elapsed >= _window) {
                ResetChallenge();
                SetChallengeProposed();
            }
        }

        private void CheckingForInput() {
            if (_elapsed >= _window) {
                SetChallengeMissed();
            }
        }

        private void AttackKeyHit(AttackChallengeInputEvent attackChallengeInputEventProps) {
            if (_complete) return;
            if (attackChallengeInputEventProps.Channel == _activeChallenge) {
                SetChallengeHit();
            } else {
                SetChallengeMissed();
            }
        }

        void ResetChallenge() {
            attack.sprite = null;
            challenges[_activeChallenge].ResetChallenge();
        }

        void SetChallengeProposed() {
            _activeChallenge = Random.Range(0, challenges.Count);
            challenges[_activeChallenge].SetChallengeProposed(attack, _proposedColor);
            SetNewWindow();
            _complete = false;
            _challengeCount++;
            _elapsed = 0;
        }

        void SetChallengeHit() {
            int performed = (int)(100 * (1 + (1 - _elapsed / _window)));
            _complete = true;
            challenges[_activeChallenge].SetChallengeHit(attack, _hitColor);
            EventBus<PlayAudioEvent>.Publish(new PlayAudioEvent {
                SoundEffectData = attackSound
            });
            EventBus<HitEvent>.Publish(new HitEvent {
                Gained = performed
            });
            _elapsed = 0;
            _window *= .75f;
        }

        void SetChallengeMissed() {
            _complete = true;
            _elapsed = 0;
            _window *= .75f;
            challenges[_activeChallenge].SetChallengeMissed(attack, _missedColor);
            EventBus<PlayAudioEvent>.Publish(new PlayAudioEvent {
                SoundEffectData = attackSound
            });
            EventBus<MissEventStatsUpdate>.Publish(new MissEventStatsUpdate());
            EventBus<MissEventHealthUpdate>.Publish(new MissEventHealthUpdate());
        }
    }
}
