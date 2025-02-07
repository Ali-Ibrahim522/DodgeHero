using System;
using Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Levels.Scarecrow {
    [Serializable]
    public class AttackChallenge {
        [SerializeField] private SpriteRenderer arrow;
        [SerializeField] private InputActionReference key;
        [SerializeField] private Sprite pre;
        [SerializeField] private Sprite post;
        [SerializeField] private int channel;
        public void EnableAttackChallenge() {
            key.action.started += OnKeyHit;
            ResetChallenge();
        }

        public void DisableAttackChallenge() {
            key.action.started -= OnKeyHit;
            ResetChallenge();
        }
        
        private void OnKeyHit(InputAction.CallbackContext obj) {
            EventBus<AttackChallengeInputEvent>.Publish(new AttackChallengeInputEvent {
                Channel = channel,
            });
        }

        public void ResetChallenge() {
            arrow.color = Color.white;
        }

        public void SetChallengeProposed(SpriteRenderer attack, Color proposed) {
            attack.sprite = pre;
            arrow.color = proposed;
        }

        public void SetChallengeHit(SpriteRenderer attack, Color hit) {
            attack.sprite = post;
            arrow.color = hit;
        }

        public void SetChallengeMissed(SpriteRenderer attack, Color missed) {
            attack.sprite = post;
            arrow.color = missed;
        }
    }
}