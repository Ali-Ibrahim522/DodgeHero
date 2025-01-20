using Events;
using UnityEngine;

namespace Levels {
    [System.Serializable]
    public class TargetChallenge : MonoBehaviour {
        [SerializeField] private GameObject target;
        [SerializeField] private SpriteRenderer targetIndicator;
        [SerializeField] private GameObject stump;
        public bool done;
        
        void OnEnable() => done = false;

        void OnMouseDown() {
            if (!done) SetTargetHit();
        }

        public void SetTargetHit() {
            EventBus<TargetHitEvent>.Publish(new TargetHitEvent());
            targetIndicator.color = Color.green;
            done = true;
        }

        public void SetTargetMissed() {
            targetIndicator.color = Color.red;
            done = true;
        }

        public void SetTargetProposed() {
            targetIndicator.color = Color.cyan;
            stump.SetActive(false);
            target.SetActive(true);
        }

        public void SetStump() {
            targetIndicator.color = Color.white;
            target.SetActive(false);
            stump.SetActive(true);
        }
    }
}
