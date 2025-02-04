using Events;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerSettings {
    public class ChangeUsernameHandler : MonoBehaviour {
        [SerializeField] private TMP_Text changeUsernameErrorText;
        [SerializeField] private TMP_InputField changeUsernameInputField;
        private string _originalUsername;
        private EventProcessor<LogChangesEvent> _onLogChangesEventProcessor;
        private EventProcessor<SaveSettingsEvent> _onSaveSettingsEventProcessor;
        private EventProcessor<ExitSettingsEvent> _onExitSettingsEventProcessor;

        private void Awake() {
            _onLogChangesEventProcessor = new EventProcessor<LogChangesEvent>(OnLogChanges);
            _onSaveSettingsEventProcessor = new EventProcessor<SaveSettingsEvent>(OnSaveSettings);
            _onExitSettingsEventProcessor = new EventProcessor<ExitSettingsEvent>(OnExitSettings);
            EventBus<LogChangesEvent>.Subscribe(_onLogChangesEventProcessor);
            EventBus<ExitSettingsEvent>.Subscribe(_onExitSettingsEventProcessor);
            OnExitSettings();
        }

        private void OnDestroy() {
            EventBus<SaveSettingsEvent>.Unsubscribe(_onSaveSettingsEventProcessor);
            EventBus<LogChangesEvent>.Unsubscribe(_onLogChangesEventProcessor);
            EventBus<ExitSettingsEvent>.Unsubscribe(_onExitSettingsEventProcessor);
        }

        public void OnExitSettings() {
            changeUsernameErrorText.text = "";
            _originalUsername = GameStateManager.Instance.GetDisplayName();
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
            GameStateManager.Instance.SetDisplayName(changeUsernameInputField.text);
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
