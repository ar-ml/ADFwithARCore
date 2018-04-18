using UnityEngine;
using UnityEngine.UI;

namespace ARaction
{
    public class MovingWindowSmoothingSlider : MonoBehaviour
    {
        [SerializeField] private Slider windowSizeSlider = null;
        [SerializeField] private MovingWindowSmoothing smoothing = null;

        public void OnEnable()
        {
            windowSizeSlider.value = smoothing.WindowSize;
        }
    }
}