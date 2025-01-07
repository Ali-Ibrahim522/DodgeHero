using JetBrains.Annotations;
using UnityEngine;

namespace Levels
{
    [System.Serializable]
    public class AttackChallenge
    {
        [SerializeField] [CanBeNull] private SpriteRenderer arrow;
        [SerializeField] private KeyCode key;
        [SerializeField] [CanBeNull] private Sprite pre;
        [SerializeField] [CanBeNull] private Sprite post;

        public void ResetChallenge() {
            if (arrow) arrow.color = Color.white;
        }

        public bool IsChallengePressed() => Input.GetKeyDown(key);

        public void SetChallengeProposed(SpriteRenderer attack) {
            if (pre) attack.sprite = pre;
            if (arrow) arrow.color = Color.cyan;
        }

        public void SetChallengeHit(SpriteRenderer attack) {
            if (post) attack.sprite = post;
            if (arrow) arrow.color = Color.green;
        }

        public void SetChallengeMissed(SpriteRenderer attack) {
            if (post) attack.sprite = post;
            if (arrow) arrow.color = Color.red;
        }
    }
}