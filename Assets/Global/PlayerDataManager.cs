using Events;
using PlayerSettings;
using PlayFab;
using PlayFab.DataModels;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Global {
    public class PlayerDataManager : MonoBehaviour {
        public static PlayerDataManager Instance { get; private set; }
        public InputActionAsset keybinds;
        public VisualSettings visuals;
        public const string KeybindSettingsName = "KeybindSettings";
        public const string VisualSettingsName = "VisualSettings";
        private void Awake() {
            if (Instance != null && Instance != this) Destroy(this);
            else Instance = this;
            visuals.SetDefaults();
        }

        public void ResetPlayerData() {
            keybinds.RemoveAllBindingOverrides();
            visuals.SetDefaults();
        }

        public void RetrievePlayerData() {
            PlayFabDataAPI.GetObjects(new GetObjectsRequest {
                Entity = new EntityKey {
                    Id = PlayFabSettings.staticPlayer.EntityId,
                    Type = PlayFabSettings.staticPlayer.EntityType
                },
                EscapeObject = true
            }, OnGetObjectsSuccess, OnPlayFabError);
        }
        private void OnGetObjectsSuccess(GetObjectsResponse obj) {
            foreach ((string objName, ObjectResult jsonResult) in obj.Objects) {
                switch (objName) {
                    case KeybindSettingsName:
                        keybinds.LoadBindingOverridesFromJson(jsonResult.EscapedDataObject);
                        break;
                    case VisualSettingsName:
                        JsonUtility.FromJsonOverwrite(jsonResult.EscapedDataObject, visuals);
                        break;
                }
            }
            EventBus<ResetSettingDefaultsEvent>.Publish(new ResetSettingDefaultsEvent());
        }

        public string GetKeybind(string map, string action) => InputControlPath.ToHumanReadableString(
            keybinds.FindActionMap(map)[action].bindings[0].effectivePath, 
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    
        private void OnPlayFabError(PlayFabError err) {
            Debug.Log(err.ApiEndpoint);
            Debug.Log(err.GenerateErrorReport());
        }
    }
}