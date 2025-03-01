using Audio;
using Events;
using Global;
using TMPro;
using UnityEngine;

namespace Start
{
    public class StartView : MonoBehaviour {
        [SerializeField] private MusicData startMusic;
        [SerializeField] private TMP_Text displayNameTxt;
        private void OnEnable() {
            EventBus<PlayMusicEvent>.Publish(new PlayMusicEvent {
                MusicData = startMusic
            });
            displayNameTxt.text = PlayerAuthManager.GetDisplayName();
        }
    }
}
