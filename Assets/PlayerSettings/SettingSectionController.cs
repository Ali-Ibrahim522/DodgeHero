using Events;
using UnityEngine;

namespace PlayerSettings {
    public class SettingSectionController : MonoBehaviour {
        [SerializeField] private GameObject settingSection;
        [SerializeField] private bool isDefault;
        
        private EventProcessor<DisableSelectedSectionEvent> _onDisableSelectedSectionEventProcessor;

        private void Awake() => _onDisableSelectedSectionEventProcessor = new EventProcessor<DisableSelectedSectionEvent>(DisableSection);
        
        private void OnEnable() {
            if (isDefault) OnSectionButtonClicked();
        }

        private void OnDisable() {
            settingSection.SetActive(false);
            EventBus<DisableSelectedSectionEvent>.Unsubscribe(_onDisableSelectedSectionEventProcessor);
        }

        public void OnSectionButtonClicked() {
            EventBus<DisableSelectedSectionEvent>.Publish(new DisableSelectedSectionEvent());
            settingSection.SetActive(true);
            EventBus<DisableSelectedSectionEvent>.Subscribe(_onDisableSelectedSectionEventProcessor);
        }
        
        private void DisableSection() => settingSection.SetActive(false);
    }
}
