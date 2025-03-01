using Audio;
using Events;
using UnityEngine;

namespace Global {
    public class UISfxController : MonoBehaviour {
        [SerializeField] private SoundEffectData onHoverSound;
        [SerializeField] private SoundEffectData onClickSound;
        
        public void OnButtonHover() => EventBus<PlayAudioEvent>.Publish(new PlayAudioEvent {
                SoundEffectData = onHoverSound,
            });
        
        public void OnButtonClick() => EventBus<PlayAudioEvent>.Publish(new PlayAudioEvent {
                SoundEffectData = onClickSound,
            });
    }
}
