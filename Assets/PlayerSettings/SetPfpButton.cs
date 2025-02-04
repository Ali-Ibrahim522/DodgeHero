using Events;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerSettings {
    public class SetPfpButton : MonoBehaviour {
        private string _pfpFileName;
        private Texture _pfpTexture;
        private void Awake() {
            _pfpTexture = GetComponent<RawImage>().texture;
            _pfpFileName = _pfpTexture.name;
        } 
        
        public void OnPfpSelected() => EventBus<PfpSelectedEvent>.Publish(new PfpSelectedEvent {
            SelectedTexture = _pfpTexture,
            FileName = _pfpFileName
        });
    }
}
