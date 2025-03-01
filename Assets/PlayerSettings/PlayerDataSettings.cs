using System.Collections.Generic;
using Events;
using Global;
using PlayFab;
using PlayFab.DataModels;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSettings {
    public class PlayerDataSettings : MonoBehaviour {
        [SerializeField] private GameObject newKeybindModal;
        [SerializeField] private TMP_Text newKeybindText;
        
        private EventProcessor<SaveSettingsEvent> _onSaveSettingsEventProcessor;
        private EventProcessor<EnableKeybindModalEvent> _onEnableKeybindModalEventProcessor;
        private EventProcessor<DisableKeybindModalEvent> _onDisableKeybindModalEventProcessor;

        private void Awake() {
            _onSaveSettingsEventProcessor = new EventProcessor<SaveSettingsEvent>(SavePlayerSettings);
            _onEnableKeybindModalEventProcessor = new EventProcessor<EnableKeybindModalEvent>(EnableKeybindModal);
            _onDisableKeybindModalEventProcessor = new EventProcessor<DisableKeybindModalEvent>(DisableKeybindModal);
        }
        private void OnEnable() {
            DisableKeybindModal();
            EventBus<SaveSettingsEvent>.Subscribe(_onSaveSettingsEventProcessor);
            EventBus<EnableKeybindModalEvent>.Subscribe(_onEnableKeybindModalEventProcessor);
            EventBus<DisableKeybindModalEvent>.Subscribe(_onDisableKeybindModalEventProcessor);
        }

        private void OnDisable() {
            EventBus<SaveSettingsEvent>.Unsubscribe(_onSaveSettingsEventProcessor);
            EventBus<EnableKeybindModalEvent>.Unsubscribe(_onEnableKeybindModalEventProcessor);
            EventBus<DisableKeybindModalEvent>.Unsubscribe(_onDisableKeybindModalEventProcessor);
        }

        private void SavePlayerSettings() {
            List<SetObject> updatedData = new ();
            if (EventBus<LoadVisualSettingEvent>.SubscriberCount() > 0) {
                EventBus<LoadVisualSettingEvent>.Publish(new LoadVisualSettingEvent());
                updatedData.Add(new SetObject {
                    ObjectName = PlayerDataManager.VisualSettingsName,
                    EscapedDataObject = JsonUtility.ToJson(PlayerDataManager.Instance.visuals)
                });
            }
            if (EventBus<LoadKeybindSettingEvent>.SubscriberCount() > 0) {
                EventBus<LoadKeybindSettingEvent>.Publish(new LoadKeybindSettingEvent());
                updatedData.Add(new SetObject {
                    ObjectName = PlayerDataManager.KeybindSettingsName,
                    EscapedDataObject = PlayerDataManager.Instance.keybinds.SaveBindingOverridesAsJson()
                });
            }
            if (EventBus<LoadAuditorySettingEvent>.SubscriberCount() > 0) {
                EventBus<LoadAuditorySettingEvent>.Publish(new LoadAuditorySettingEvent());
                updatedData.Add(new SetObject {
                    ObjectName = PlayerDataManager.AuditorySettingsName,
                    EscapedDataObject = JsonUtility.ToJson(PlayerDataManager.Instance.auditory)
                });
            }
            if (updatedData.Count == 0) return;
            PlayFabDataAPI.SetObjects(new SetObjectsRequest {
                Entity = new EntityKey {
                    Id = PlayFabSettings.staticPlayer.EntityId,
                    Type = PlayFabSettings.staticPlayer.EntityType
                },
                Objects = updatedData
            }, null, (e) => Debug.Log(e.GenerateErrorReport()));
        }

        private void EnableKeybindModal(EnableKeybindModalEvent enableKeybindModalEventProps) {
            newKeybindText.text = enableKeybindModalEventProps.KeyName;
            newKeybindModal.SetActive(true);
        }
        
        private void DisableKeybindModal() {
            newKeybindModal.SetActive(false);
        }
    }
}
