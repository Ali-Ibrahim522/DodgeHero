using System.Collections;
using Audio;
using Events;
using Global;
using UnityEngine;

namespace Levels.Scarecrow {
    public class CrowChallenge : MonoBehaviour {
        [SerializeField] private Sprite perchSprite;
        [SerializeField] private Sprite flySprite;
        [SerializeField] private SpriteRenderer crowRender;
        [SerializeField] private float perchAt;
    
        private CubicPath _crowPath;
        private int _channel;
        private float _speed;
        private float _perchSpeed;
        private float _flySpeed;
        private Quaternion _crowRotation;

        private ElapsedContainer _elapsedContainer;
        private CrowState _crowState;
        private Vector3 _scaredStart;
        private Vector3 _scaredEnd;
        
        private EventProcessor<DeathEvent> _onDeathEventProcessor;
        private EventProcessor<CrowScaredEvent> _onCrowScaredEventProcessor;
    
        private enum CrowState {
            Scared,
            Swooping,
            Perching,
            Destroyed
        }

        void Awake() {
            _elapsedContainer = new ElapsedContainer();
            _onDeathEventProcessor = new EventProcessor<DeathEvent>(DestroyCrow);
            _onCrowScaredEventProcessor = new EventProcessor<CrowScaredEvent>(OnCrowScared);
        }

        void OnEnable() {
            EventBus<DeathEvent>.Subscribe(_onDeathEventProcessor);
        }
    
        void OnDisable() => EventBus<CrowScaredEvent>.Unsubscribe(_onCrowScaredEventProcessor, _channel);

        private void OnDestroy() => EventBus<DeathEvent>.Unsubscribe(_onDeathEventProcessor);

        void Update() {
            _elapsedContainer.elapsed += Time.deltaTime * _speed;
            switch (_crowState) {
                case CrowState.Scared:
                    if (_elapsedContainer.elapsed < 1f) transform.position = Vector3.Lerp(_scaredStart, _scaredEnd, _elapsedContainer.elapsed);
                    else Destroy(gameObject);
                    break;
                case CrowState.Swooping:
                    if (perchAt <= _elapsedContainer.elapsed) StartCrowPerching();
                    transform.position = _crowPath.GetPositionOnCurve(_elapsedContainer.elapsed);
                    break;
                case CrowState.Perching:
                    if (_elapsedContainer.elapsed < 1f) _speed = Mathf.Lerp(_flySpeed, _perchSpeed, (_elapsedContainer.elapsed - perchAt) * 3.33f);
                    else StartCoroutine(StartMissedCrow());
                    transform.position = _crowPath.GetPositionOnCurve(_elapsedContainer.elapsed);
                    break;
            }
        }

        void DestroyCrow() {
            if (_crowState == CrowState.Perching) {
                EventBus<RemoveCrowEvent>.Publish(new RemoveCrowEvent {
                    Channel = _channel,
                });
            }
            Destroy(gameObject);
        }

        public void InitCrow(int channel, CubicPath crowPath, float speed, Quaternion crowRotation) {
            _crowRotation = crowRotation;
            _flySpeed = speed;
            _speed = speed;
            _perchSpeed = speed / 10f;
            _crowPath = crowPath;
            _channel = channel;
            crowRender.flipX = _crowPath.GetStart().x < _crowPath.GetDestination().x;
            crowRender.sprite = flySprite;
            _elapsedContainer.elapsed = 0;
            EventBus<CrowScaredEvent>.Subscribe(_onCrowScaredEventProcessor, _channel);
            _crowState = CrowState.Swooping;
        }

        private IEnumerator StartMissedCrow() {
            _crowState = CrowState.Destroyed;
            EventBus<RemoveCrowEvent>.Publish(new RemoveCrowEvent {
                Channel = _channel,
            });
            EventBus<MissEventHealthUpdate>.Publish(new MissEventHealthUpdate());
            EventBus<MissEventStatsUpdate>.Publish(new MissEventStatsUpdate());
            crowRender.color = PlayerDataManager.Instance.visuals.missed;
            yield return new WaitForSeconds(.75f);
            Destroy(gameObject);
        }

        private void StartCrowPerching() {
            _crowState = CrowState.Perching;
            transform.rotation = _crowRotation;
            crowRender.sprite = perchSprite;
            crowRender.color = PlayerDataManager.Instance.visuals.proposed;
            EventBus<CrowPerchingEvent>.Publish(new CrowPerchingEvent {
                Channel = _channel,
                Progress = _elapsedContainer
            });
        }

        private void OnCrowScared() {
            int performed = (int)(100 * (1 + (1 - (_elapsedContainer.elapsed - .7f) / .3f)));
            EventBus<HitEvent>.Publish(new HitEvent {
                Gained = performed
            });
            crowRender.color = PlayerDataManager.Instance.visuals.hit;
            transform.rotation = new Quaternion(0, 0, 0, 0);
            _crowState = CrowState.Scared;
            _scaredStart = transform.position;
            _scaredEnd = transform.position;
            crowRender.sprite = flySprite;
            crowRender.flipX = !crowRender.flipX;
            _scaredEnd.x = _crowPath.GetStart().x;
            _elapsedContainer.elapsed = 0f;
            _speed = _flySpeed;
        }
    }
}
