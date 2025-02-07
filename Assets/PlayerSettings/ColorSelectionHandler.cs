using UnityEngine;
using UnityEngine.UI;

namespace PlayerSettings {
    public class ColorSelectionHandler : MonoBehaviour {
        [SerializeField] private Slider redSlider;
        [SerializeField] private Slider greenSlider;
        [SerializeField] private Slider blueSlider;
        [SerializeField] private Image colorDisplay;
        protected Color SelectedColor;
        protected Color OriginalColor;

        public void OnRedChanged() {
            SelectedColor.r = redSlider.value;
            colorDisplay.color =SelectedColor;
        }
        public void OnGreenChanged() {
            SelectedColor.g = greenSlider.value;
            colorDisplay.color = SelectedColor;
        }
        public void OnBlueChanged() {
            SelectedColor.b = blueSlider.value;
            colorDisplay.color = SelectedColor;
        }
        
        protected void ResetColorSelection() {
            SelectedColor = OriginalColor;
            colorDisplay.color = SelectedColor;
            redSlider.value = OriginalColor.r;
            greenSlider.value = OriginalColor.g;
            blueSlider.value = OriginalColor.b;
        }
    }
}
