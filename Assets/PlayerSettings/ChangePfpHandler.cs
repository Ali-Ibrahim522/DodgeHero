using Events;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerSettings {
    public class ChangePfpHandler : MonoBehaviour {
        [SerializeField] private GameObject pfpSelectionContainer;
        [SerializeField] private TMP_Text changePfpErrorText;
        private RawImage _selectedPfpImage;
        private string _originalPfpFileName;
        private string _selectedPfpFileName;
        private EventProcessor<PfpSelectedEvent> _onPfpSelectedEventProcessor;
        private EventProcessor<LogChangesEvent> _onLogChangesEventProcessor;
        private EventProcessor<SaveSettingsEvent> _onSaveSettingsEventProcessor;
        private EventProcessor<ExitSettingsEvent> _onExitSettingsEventProcessor;

        private void Awake() {
            _selectedPfpImage = GetComponent<RawImage>();
            _onLogChangesEventProcessor = new EventProcessor<LogChangesEvent>(OnLogChanges);
            _onSaveSettingsEventProcessor = new EventProcessor<SaveSettingsEvent>(OnSaveSettings);
            _onPfpSelectedEventProcessor = new EventProcessor<PfpSelectedEvent>(OnPfpSelected);
            _onExitSettingsEventProcessor = new EventProcessor<ExitSettingsEvent>(OnExitSettings);
            EventBus<LogChangesEvent>.Subscribe(_onLogChangesEventProcessor);
            EventBus<ExitSettingsEvent>.Subscribe(_onExitSettingsEventProcessor);
            OnExitSettings();
        }

        private void OnEnable() => EventBus<PfpSelectedEvent>.Subscribe(_onPfpSelectedEventProcessor);
        

        private void OnDisable() => EventBus<PfpSelectedEvent>.Unsubscribe(_onPfpSelectedEventProcessor);

        private void OnDestroy() {
            EventBus<SaveSettingsEvent>.Unsubscribe(_onSaveSettingsEventProcessor);
            EventBus<LogChangesEvent>.Unsubscribe(_onLogChangesEventProcessor);
            EventBus<ExitSettingsEvent>.Unsubscribe(_onExitSettingsEventProcessor);
        }

        private void OnExitSettings() {
            _originalPfpFileName = GameStateManager.Instance.GetPfpFileName();
            _selectedPfpFileName = _originalPfpFileName;
            _selectedPfpImage.texture = GameStateManager.Instance.GetPfp();
            changePfpErrorText.text = "";
        }

        public void OnChangePfpButtonClicked() => pfpSelectionContainer.SetActive(!pfpSelectionContainer.activeSelf);

        private void OnPfpSelected(PfpSelectedEvent pfpSelectedEventProps) {
            _selectedPfpImage.texture = pfpSelectedEventProps.SelectedTexture;
            _selectedPfpFileName = pfpSelectedEventProps.FileName;
        }

        private void OnLogChanges() {
            if (_originalPfpFileName == _selectedPfpFileName) {
                EventBus<SaveSettingsEvent>.Unsubscribe(_onSaveSettingsEventProcessor);
                return;
            }
            EventBus<SaveSettingsEvent>.Subscribe(_onSaveSettingsEventProcessor);
            EventBus<AddChangeEvent>.Publish(new AddChangeEvent {
                ChangeLog = $"Pfp: {_originalPfpFileName} -> {_selectedPfpFileName}\n"
            });
        }

        private void OnSaveSettings() {
            PlayFabClientAPI.UpdateAvatarUrl(new UpdateAvatarUrlRequest {
                ImageUrl = _selectedPfpFileName
            }, OnUpdateAvatarUrlSuccess, UpdateAvatarUrlError);
        }
        private void OnUpdateAvatarUrlSuccess(EmptyResponse suc) {
            changePfpErrorText.text = "";
            _originalPfpFileName = _selectedPfpFileName;
            StartCoroutine(GameStateManager.Instance.SetPlayerPfp(_selectedPfpFileName));
        }
        private void UpdateAvatarUrlError(PlayFabError err) => changePfpErrorText.text = "There was an error updating your pfp, please wait and try again.";
    }
}
