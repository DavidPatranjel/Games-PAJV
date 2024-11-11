using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


namespace PlayerManager
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthText; // Drag and drop the TMP_Text in the inspector
        [SerializeField] private Slider _healthSlider;
        private Coroutine _flickerCoroutine;
        // Flicker parameters as class members
        private float _flickerSpeed = 2f;
        private float _minAlpha = 0.3f;
        private float _maxAlpha = 1f;
        [SerializeField] Graphic uiElement;
        public void HandlePlayerHealthChanged(int health)
        {
            // Update the TMP_Text with the current health
            _healthText.text = health.ToString() + "HP";
            // Update the Slider's value to reflect the current health
            _healthSlider.value = health;
        }
        public void StartFlickerAnimation()
        {
            if (_flickerCoroutine != null)
                StopCoroutine(_flickerCoroutine);

            _flickerCoroutine = StartCoroutine(FlickerAnimation());
        }

        public void StopFlickerAnimation()
        {
            if (_flickerCoroutine != null)
            {
                StopCoroutine(_flickerCoroutine);
                _flickerCoroutine = null;
                uiElement.color = new Color(uiElement.color.r, uiElement.color.g, uiElement.color.b, _maxAlpha);
            }
        }

        private IEnumerator FlickerAnimation()
        {
            Color originalColor = uiElement.color;
            float alpha = _maxAlpha;

            while (true)
            {
                // Lerp alpha between minAlpha and maxAlpha
                alpha = Mathf.PingPong(Time.time * _flickerSpeed, _maxAlpha - _minAlpha) + _minAlpha;
                uiElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

                yield return null;
            }
        }


    }
}
