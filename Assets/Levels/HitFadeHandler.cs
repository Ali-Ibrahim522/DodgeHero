using UnityEngine;
using UnityEngine.InputSystem;

namespace Levels {
    public class HitFadeHandler : MonoBehaviour {
        [SerializeField] private InputActionReference inputAction;
        [SerializeField] private SpriteRenderer hitFadeRenderer;
        [SerializeField] private Transform hitFadeTransform;
        [SerializeField] private float hitFadeSpeed;
        [SerializeField] private float startScale;
        [SerializeField] private float endScale;
        [SerializeField] private float startAlpha;
        [SerializeField] private float endAlpha;
        private float _fade;
        private Color _fadeColor;
        private Vector3 _startScaleV3;
        private Vector3 _endScaleV3;

        private void OnEnable() {
            SetupHitFade();
            inputAction.action.started += StartHitFade;
        }
        
        private void OnDisable() => inputAction.action.started -= StartHitFade;
        
        private void StartHitFade(InputAction.CallbackContext obj) {
            _fade = 0;
            hitFadeRenderer.enabled = true;
        }

        private void Update() {
            if (_fade < 1f) {
                _fade += Time.deltaTime * hitFadeSpeed;
                _fadeColor.a = Mathf.Lerp(startAlpha, endAlpha, _fade);
                hitFadeRenderer.color = _fadeColor;
                hitFadeTransform.localScale = Vector3.Lerp(_startScaleV3, _endScaleV3, _fade);
                if (_fade >= 1f) hitFadeRenderer.enabled = false;
            }
        }

        private void SetupHitFade() {
            _startScaleV3 = new Vector3(startScale, startScale, 1f);
            _endScaleV3 = new Vector3(endScale, endScale, 1f);
            _fade = endAlpha;
            _fadeColor = hitFadeRenderer.color;
            hitFadeRenderer.enabled = false;
        }

    }
}
