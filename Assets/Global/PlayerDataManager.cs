using Events;
using PlayFab;
using PlayFab.DataModels;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

namespace Global {
    public class PlayerDataManager : MonoBehaviour {
        public static PlayerDataManager Instance { get; private set; }
        
        public InputActionAsset keybinds;
        public VisualSettings visuals;
        
        public AudioMixerGroup masterMixer;
        public AudioMixerGroup musicMixer;
        public AudioMixerGroup sfxMixer;
        public AuditorySettings auditory;
        
        public const string KeybindSettingsName = "KeybindSettings";
        public const string VisualSettingsName = "VisualSettings";
        public const string AuditorySettingsName = "AuditorySettings";
        
        private void Awake() {
            if (Instance != null && Instance != this) Destroy(this);
            else Instance = this;
            ResetPlayerData();
        }

        public void ResetPlayerData() {
            keybinds.RemoveAllBindingOverrides();
            visuals.SetDefaults();
            auditory.SetDefaults();
            SetVolumes();
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
                    case AuditorySettingsName:
                        JsonUtility.FromJsonOverwrite(jsonResult.EscapedDataObject, auditory);
                        SetVolumes();
                        break;
                }
            }
            EventBus<ResetSettingDefaultsEvent>.Publish(new ResetSettingDefaultsEvent());
        }

        public string GetKeybind(string map, string action) => InputControlPath.ToHumanReadableString(
            keybinds.FindActionMap(map)[action].bindings[0].effectivePath, 
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        private void SetVolumes() {
            masterMixer.audioMixer.SetFloat("MasterVolume", auditory.masterVolume);
            musicMixer.audioMixer.SetFloat("MusicVolume", auditory.musicVolume);
            sfxMixer.audioMixer.SetFloat("SfxVolume", auditory.sfxVolume);
        }
    
        private void OnPlayFabError(PlayFabError err) {
            Debug.Log(err.ApiEndpoint);
            Debug.Log(err.GenerateErrorReport());
        }
    }
}