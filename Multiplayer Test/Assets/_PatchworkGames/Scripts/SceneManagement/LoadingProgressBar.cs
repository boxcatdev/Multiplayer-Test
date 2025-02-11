using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PatchworkGames
{
    public class LoadingProgressBar : MonoBehaviour
    {
        private Image _image;
        private Slider _slider;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _slider = GetComponent<Slider>();
        }
        private void Update()
        {
            if (_image != null)
            {
                _image.fillAmount = Loader.GetLoadingProgress();
            }
            if (_slider != null)
            {
                _slider.maxValue = 1f;
                _slider.value = Loader.GetLoadingProgress();
            }
        }
    }
}
