using Events;
using Global;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

namespace PlayerSettings {
    public class ChangeUsernameHandler : MonoBehaviour {
        [SerializeField] private TMP_Text changeUsernameErrorText;
        [SerializeField] private TMP_InputField changeUsernameInputField;
        private string _originalUsername;
        
        private EventProcessor<LogChangesEvent> _onLogChangesEventProcessor;
        private EventProcessor<ResetSettingDefaultsEvent> _onResetSettingDefaultsEventProcessor;
        private EventProcessor<SaveSettingsEvent> _onSaveSettingsEventProcessor;

        private void Awake() {
            _onLogChangesEventProcessor = new EventProcessor<LogChangesEvent>(OnLogChanges);
            _onSaveSettingsEventProcessor = new EventProcessor<SaveSettingsEvent>(OnSaveSettings);
            _onResetSettingDefaultsEventProcessor = new EventProcessor<ResetSettingDefaultsEvent>(OnExitSettings);
            EventBus<LogChangesEvent>.Subscribe(_onLogChangesEventProcessor);
            EventBus<ResetSettingDefaultsEvent>.Subscribe(_onResetSettingDefaultsEventProcessor);
            OnExitSettings();
        }

        private void OnDestroy() {
            EventBus<SaveSettingsEvent>.Unsubscribe(_onSaveSettingsEventProcessor);
            EventBus<LogChangesEvent>.Unsubscribe(_onLogChangesEventProcessor);
            EventBus<ResetSettingDefaultsEvent>.Unsubscribe(_onResetSettingDefaultsEventProcessor);
        }

        public void OnExitSettings() {
            changeUsernameErrorText.text = "";
            _originalUsername = PlayerAuthManager.GetDisplayName();
            changeUsernameInputField.text = _originalUsername;
        }

        private void OnLogChanges() {
            if (_originalUsername == changeUsernameInputField.text) {
                EventBus<SaveSettingsEvent>.Unsubscribe(_onSaveSettingsEventProcessor);
                return;
            }
            EventBus<SaveSettingsEvent>.Subscribe(_onSaveSettingsEventProcessor);
            EventBus<AddChangeEvent>.Publish(new AddChangeEvent {
                ChangeLog = $"Username: {_originalUsername} -> {changeUsernameInputField.text}\n"
            });
        }

        private void OnSaveSettings() {
            PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest {
                DisplayName = changeUsernameInputField.text
            }, OnUpdateUsernameSuccess, OnUpdateUsernameError);
        }
        private void OnUpdateUsernameSuccess(UpdateUserTitleDisplayNameResult suc) {
            changeUsernameErrorText.text = "";
            _originalUsername = changeUsernameInputField.text;
            PlayerAuthManager.SetDisplayName(changeUsernameInputField.text);
            EventBus<SaveSettingsEvent>.Unsubscribe(_onSaveSettingsEventProcessor);
        }
        private void OnUpdateUsernameError(PlayFabError err) {
            changeUsernameErrorText.text = err.Error switch {
                PlayFabErrorCode.NameNotAvailable => "Username is taken",
                PlayFabErrorCode.InvalidParams => "Invalid username",
                PlayFabErrorCode.EmailAddressNotAvailable => "Invalid username",
                _ => "There was an error updating your username."
            };
        }
    }
}
