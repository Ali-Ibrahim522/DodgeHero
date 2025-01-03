using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    private int _heart;
    public List<Image> hearts;
    private EventProcessor<MissEventHealthUpdate> _onMissProcessor;

    void Awake() {
        _onMissProcessor = new EventProcessor<MissEventHealthUpdate>(OnMiss);
    }

    void OnEnable() {
        _heart = 0;
        foreach (Image h in hearts) h.color = Color.white;
        EventBus<MissEventHealthUpdate>.Subscribe(_onMissProcessor);
    }
    void OnDisable() {
        EventBus<MissEventHealthUpdate>.Unsubscribe(_onMissProcessor);
    }

    public void OnMiss() {
        hearts[_heart++].color = Color.red;
        if (_heart == hearts.Count) EventBus<DeathEvent>.Publish(new DeathEvent());
    }
}
