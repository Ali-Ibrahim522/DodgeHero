using Events;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSettings {
    public class KeybindSelection : MonoBehaviour {
        [SerializeField] private TMP_Text keybindText;
        [SerializeField] private InputActionReference inputAction;
        private InputAction _selectedInputAction;
        private InputActionRebindingExtensions.RebindingOperation _rebindOperation;
        
        private EventProcessor<LogChangesEvent> _onLogChangesEventProcessor;
        private EventProcessor<ResetSettingDefaultsEvent> _onResetSettingDefaultsEventProcessor;
        private EventProcessor<LoadKeybindSettingEvent> _onLoadKeybindSettingEventProcessor;
        
        private void Awake() {
            OnExitSettings();
            _onLogChangesEventProcessor = new EventProcessor<LogChangesEvent>(AddToLoadEvent);
            _onResetSettingDefaultsEventProcessor = new EventProcessor<ResetSettingDefaultsEvent>(OnExitSettings);
            _onLoadKeybindSettingEventProcessor = new EventProcessor<LoadKeybindSettingEvent>(LoadKeybindSetting);
            EventBus<LogChangesEvent>.Subscribe(_onLogChangesEventProcessor);
            EventBus<ResetSettingDefaultsEvent>.Subscribe(_onResetSettingDefaultsEventProcessor);
        }

        private void OnDestroy() {
            EventBus<LogChangesEvent>.Unsubscribe(_onLogChangesEventProcessor);
            EventBus<ResetSettingDefaultsEvent>.Unsubscribe(_onResetSettingDefaultsEventProcessor);
        }

        public void OnRebindClicked() {
            EventBus<EnableKeybindModalEvent>.Publish(new EnableKeybindModalEvent {
                KeyName = inputAction.action.name,
            });
            _rebindOperation = _selectedInputAction.PerformInteractiveRebinding()
                .WithCancelingThrough("<Keyboard>/escape")
                .OnMatchWaitForAnother(.1f)
                .OnCancel(ResetKeybinding)
                .OnComplete(ResetKeybinding)
                .Start();
        }

        private void ResetKeybinding(InputActionRebindingExtensions.RebindingOperation obj) {
            _rebindOperation.Dispose();
            keybindText.text = ToReadable(_selectedInputAction.bindings[0].effectivePath);
            EventBus<DisableKeybindModalEvent>.Publish(new DisableKeybindModalEvent());
        }

        private void OnExitSettings() {
            _selectedInputAction = new InputAction(binding: inputAction.action.bindings[0].effectivePath);
            keybindText.text = ToReadable(_selectedInputAction.bindings[0].effectivePath);
        }
        
        private void AddToLoadEvent() {
            string selectedIb = _selectedInputAction.bindings[0].effectivePath;
            string originalIb = inputAction.action.bindings[0].effectivePath;
            if (selectedIb != originalIb) {
                EventBus<AddChangeEvent>.Publish(new AddChangeEvent {
                    ChangeLog = $"{inputAction.action.name}: {ToReadable(originalIb)} -> {ToReadable(selectedIb)}\n"
                });
                EventBus<LoadKeybindSettingEvent>.Subscribe(_onLoadKeybindSettingEventProcessor);
            } else {
                EventBus<LoadKeybindSettingEvent>.Unsubscribe(_onLoadKeybindSettingEventProcessor);
            }
        }

        private string ToReadable(string effectivePath) => InputControlPath.ToHumanReadableString(effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        private void LoadKeybindSetting() {
            inputAction.action.ApplyBindingOverride(new InputBinding {
                overridePath = _selectedInputAction.bindings[0].effectivePath
            });
            EventBus<LoadKeybindSettingEvent>.Unsubscribe(_onLoadKeybindSettingEventProcessor);
        }

    }
}
