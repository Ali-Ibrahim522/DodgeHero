using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Levels
{
    [System.Serializable]
    public class AttackChallenge
    {
        [SerializeField] [CanBeNull] private Image arrow;
        [SerializeField] private KeyCode key;
        [SerializeField] [CanBeNull] private Sprite pre;
        [SerializeField] [CanBeNull] private Sprite post;

        public void ResetChallenge() {
            if (arrow) arrow.color = Color.white;
        }

        public bool IsChallengePressed() => Input.GetKeyDown(key);

        public void SetChallengeProposed(Image attack) {
            if (pre) attack.sprite = pre;
            if (arrow) arrow.color = Color.cyan;
        }

        public void SetChallengeHit(Image attack) {
            if (post) attack.sprite = post;
            if (arrow) arrow.color = Color.green;
        }

        public void SetChallengeMissed(Image attack) {
            if (post) attack.sprite = post;
            if (arrow) arrow.color = Color.red;
        }
    }
}