using System;
using System.Threading.Tasks;
using UnityEngine;
using PlayFab.ClientModels;
using UnityEngine.Networking;

namespace Global {
    public static class PlayerAuthManager {
        public enum PlayerState {
            None,
            Player,
            Guest,
            Deleted
        }
        
        private static Texture2D _pfp = new (2, 2, TextureFormat.BGRA32,false);
        private static PlayerProfileModel _playerDetails;
        private static PlayerState _playerState = PlayerState.None;
        
        public static PlayerState GetPlayerState() => _playerState;

        public static void Logout() {
            _playerState = PlayerState.None;
            _playerDetails = null;
        }

        public static void Login(PlayerProfileModel playerDetails) {
            _playerState = PlayerState.Player;
            _playerDetails = playerDetails;
            SetPlayerPfp(_playerDetails.AvatarUrl);
        }

        public static void LoginAsGuest() {
            _playerState = PlayerState.Guest;
            _playerDetails = null;
            SetPlayerPfp("defaultPfp");
        }
        
        public static void SetDisplayName(string displayName) => _playerDetails.DisplayName = displayName;

        public static string GetDisplayName() => _playerState == PlayerState.Guest ? "Guest" : _playerDetails.DisplayName;

        public static string GetPfpFileName() => _playerDetails.AvatarUrl;
        
        public static Texture GetPfp() => _pfp;

        public static void DeleteAccount() {
            Logout();
            _playerState = PlayerState.Deleted;
        }

        public static async void SetPlayerPfp(string pfpName) {
            try {
                await SetPfpTexture(pfpName);
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }

        private static async Task SetPfpTexture(string pfpName) {
            if (_playerState == PlayerState.Player) {
                _playerDetails.AvatarUrl = pfpName;
            }
            string filePath = $"{Application.streamingAssetsPath}/{pfpName}.jpg";
        
            #if !UNITY_WEBGL
                        filePath = "file://" + filePath;
            #endif
            using UnityWebRequest uwr = UnityWebRequest.Get(filePath);
            UnityWebRequestAsyncOperation retrieval = uwr.SendWebRequest();
            while (!retrieval.isDone) await Task.Yield();
            if (uwr.result == UnityWebRequest.Result.Success) _pfp.LoadImage(uwr.downloadHandler.data);
            else Debug.Log(uwr.error);
        }
    }
}
