using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

namespace Levels {
    public class HealthSystem : MonoBehaviour
    {
        private int _heart;
        public List<SpriteRenderer> hearts;
        private EventProcessor<MissEventHealthUpdate> _onMissProcessor;

        void Awake() {
            _onMissProcessor = new EventProcessor<MissEventHealthUpdate>(OnMiss);
        }

        void OnEnable() {
            _heart = 0;
            foreach (SpriteRenderer h in hearts) h.color = Color.white;
            EventBus<MissEventHealthUpdate>.Subscribe(_onMissProcessor);
        }
        void OnDisable() {
            EventBus<MissEventHealthUpdate>.Unsubscribe(_onMissProcessor);
        }

        public void OnMiss(MissEventHealthUpdate missEventHealthUpdateProps) {
            if (_heart < hearts.Count) {
                hearts[_heart++].color = Color.red;
                if (_heart == hearts.Count) StartCoroutine(WaitedDeath(missEventHealthUpdateProps.DeathWait));
            }
        }

        IEnumerator WaitedDeath(float waitDeath) {
            yield return new WaitForSeconds(waitDeath);
            EventBus<DeathEvent>.Publish(new DeathEvent());
        }
    }
}
