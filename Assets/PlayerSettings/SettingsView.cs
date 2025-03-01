using Audio;
using Events;
using Global;
using TMPro;
using UnityEngine;

namespace PlayerSettings {
    public class SettingsView : MonoBehaviour {
        [SerializeField] private MusicData settingsMusic;
        [SerializeField] private GameObject saveConfirmationModal;
        [SerializeField] private TMP_Text changeLog;
        
        private EventProcessor<AddChangeEvent> _onAddChangeEventProcessor;

        private void Awake() => _onAddChangeEventProcessor = new EventProcessor<AddChangeEvent>(AddChange);
        
        private void OnEnable() {
            EventBus<AddChangeEvent>.Subscribe(_onAddChangeEventProcessor);
            EventBus<PlayMusicEvent>.Publish(new PlayMusicEvent {
                MusicData = settingsMusic
            });
            OnCancelClicked();
        }

        private void OnDisable() => EventBus<AddChangeEvent>.Unsubscribe(_onAddChangeEventProcessor);

        private void AddChange(AddChangeEvent addChangeEventProps) => changeLog.text += addChangeEventProps.ChangeLog;
        
        public void OnSaveClicked() {
            EventBus<LogChangesEvent>.Publish(new LogChangesEvent());
            saveConfirmationModal.SetActive(true);
        }

        public void OnBackClicked() {
            EventBus<ResetSettingDefaultsEvent>.Publish(new ResetSettingDefaultsEvent());
            GameStateManager.Instance.MoveToState(GameStateManager.GameState.LevelSelect);
        }

        public void OnCancelClicked() {
            changeLog.text = "";
            saveConfirmationModal.SetActive(false);
        }

        public void OnConfirmClicked() {
            EventBus<SaveSettingsEvent>.Publish(new SaveSettingsEvent());
            OnCancelClicked();
        }
    }
}
