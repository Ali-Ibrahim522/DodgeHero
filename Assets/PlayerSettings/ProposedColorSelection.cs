using Events;
using Global;
using UnityEngine;

namespace PlayerSettings {
    public class ProposedColorSelection : ColorSelectionHandler {
        private EventProcessor<LogChangesEvent> _onLogChangesEventProcessor;
        private EventProcessor<ResetSettingDefaultsEvent> _onResetSettingDefaultsEventProcessor;
        private EventProcessor<LoadVisualSettingEvent> _onLoadVisualSettingEventProcessor;
        
        private void Awake() {
            OnExitSettings();
            _onLogChangesEventProcessor = new EventProcessor<LogChangesEvent>(AddToLoadEvent);
            _onResetSettingDefaultsEventProcessor = new EventProcessor<ResetSettingDefaultsEvent>(OnExitSettings);
            _onLoadVisualSettingEventProcessor = new EventProcessor<LoadVisualSettingEvent>(LoadVisualSetting);
            EventBus<LogChangesEvent>.Subscribe(_onLogChangesEventProcessor);
            EventBus<ResetSettingDefaultsEvent>.Subscribe(_onResetSettingDefaultsEventProcessor);
        }

        private void OnDestroy() {
            EventBus<LogChangesEvent>.Unsubscribe(_onLogChangesEventProcessor);
            EventBus<ResetSettingDefaultsEvent>.Unsubscribe(_onResetSettingDefaultsEventProcessor);
        }
        
        public void OnExitSettings() {
            OriginalColor = PlayerDataManager.Instance.visuals.proposed;
            EventBus<LoadVisualSettingEvent>.Unsubscribe(_onLoadVisualSettingEventProcessor);
            ResetColorSelection();
        }

        private void AddToLoadEvent() {
            if (SelectedColor != OriginalColor) {
                EventBus<AddChangeEvent>.Publish(new AddChangeEvent {
                    ChangeLog = $"Proposed Color: #{ColorUtility.ToHtmlStringRGBA(OriginalColor)} -> #{ColorUtility.ToHtmlStringRGBA(SelectedColor)}\n"
                });
                EventBus<LoadVisualSettingEvent>.Subscribe(_onLoadVisualSettingEventProcessor);
            } else EventBus<LoadVisualSettingEvent>.Unsubscribe(_onLoadVisualSettingEventProcessor);
        }

        private void LoadVisualSetting() {
            PlayerDataManager.Instance.visuals.proposed = SelectedColor;
            OnExitSettings();
        }
    }
}
