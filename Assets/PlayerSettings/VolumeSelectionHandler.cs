using UnityEngine;
using UnityEngine.UI;

namespace PlayerSettings {
    public class VolumeSelectionHandler : MonoBehaviour {
        [SerializeField] private Slider volumeSlider;
        protected float SelectedVolume;
        protected float OriginalVolume;

        protected void OnVolumeSliderChanged() => SelectedVolume = Mathf.Round(SliderToVolume(volumeSlider.value) * 1000) * .001f;

        protected void ResetVolume() {
            SelectedVolume = OriginalVolume;
            volumeSlider.value = VolumeToSlider(OriginalVolume);
        }

        private float VolumeToSlider(float volume) => Mathf.Pow(10f, volume / 20f);
        
        private float SliderToVolume(float slider) => Mathf.Log10(slider) * 20;
    }
}
