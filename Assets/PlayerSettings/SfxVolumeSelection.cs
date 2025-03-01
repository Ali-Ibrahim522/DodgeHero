using Events;
using Global;
using UnityEngine;

namespace PlayerSettings {
    public class SfxVolumeSelection : VolumeSelectionHandler {
        private EventProcessor<LogChangesEvent> _onLogChangesEventProcessor;
        private EventProcessor<ResetSettingDefaultsEvent> _onResetSettingDefaultsEventProcessor;
        private EventProcessor<LoadAuditorySettingEvent> _onLoadAuditorySettingEventProcessor;
        
        private void Awake() {
            OnExitSettings();
            _onLogChangesEventProcessor = new EventProcessor<LogChangesEvent>(AddToLoadEvent);
            _onResetSettingDefaultsEventProcessor = new EventProcessor<ResetSettingDefaultsEvent>(OnExitSettings);
            _onLoadAuditorySettingEventProcessor = new EventProcessor<LoadAuditorySettingEvent>(LoadAuditorySetting);
            EventBus<LogChangesEvent>.Subscribe(_onLogChangesEventProcessor);
            EventBus<ResetSettingDefaultsEvent>.Subscribe(_onResetSettingDefaultsEventProcessor);
        }

        private void OnDestroy() {
            EventBus<LogChangesEvent>.Unsubscribe(_onLogChangesEventProcessor);
            EventBus<ResetSettingDefaultsEvent>.Unsubscribe(_onResetSettingDefaultsEventProcessor);
        }

        public void OnVolumeChanged() {
            OnVolumeSliderChanged();
            PlayerDataManager.Instance.sfxMixer.audioMixer.SetFloat("SfxVolume", SelectedVolume);
        }
        
        public void OnExitSettings() {
            OriginalVolume = PlayerDataManager.Instance.auditory.sfxVolume;
            EventBus<LoadAuditorySettingEvent>.Unsubscribe(_onLoadAuditorySettingEventProcessor);
            ResetVolume();
        }

        private void AddToLoadEvent() {
            if (!Mathf.Approximately(SelectedVolume, OriginalVolume)) {
                EventBus<AddChangeEvent>.Publish(new AddChangeEvent {
                    ChangeLog = $"Sfx Volume: {OriginalVolume} -> {SelectedVolume}\n"
                });
                EventBus<LoadAuditorySettingEvent>.Subscribe(_onLoadAuditorySettingEventProcessor);
            } else EventBus<LoadAuditorySettingEvent>.Unsubscribe(_onLoadAuditorySettingEventProcessor);
        }

        private void LoadAuditorySetting() {
            PlayerDataManager.Instance.auditory.sfxVolume = SelectedVolume;
            OnExitSettings();
        }
    }
}
