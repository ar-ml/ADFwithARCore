using UnityEngine;
using UnityEngine.UI;

namespace ARaction
{
    [CreateAssetMenu(menuName = "ARaction/Smoothing/MovingWindowParamSetter")]
    public class MovingWindowSmoothingParamSetter : SmoothingParamSetter
    {
        public override Smoothing Smoothing { get { return m_Smoothing; } }

        [SerializeField] private Slider intSliderPrefab = null;
        [SerializeField] private MovingWindowSmoothing smoothingPrefab = null;

        private Slider m_windowSizeSlider;
        private MovingWindowSmoothing m_Smoothing;

        protected override void Init(Transform uiParent)
        {
            m_windowSizeSlider = Instantiate<Slider>(intSliderPrefab, uiParent);
            m_Smoothing = Instantiate<MovingWindowSmoothing>(smoothingPrefab);
            m_Smoothing.name = smoothingPrefab.name;
            m_windowSizeSlider.value = m_Smoothing.WindowSize;
            m_windowSizeSlider.onValueChanged.AddListener(newValue => m_Smoothing.WindowSize = Mathf.RoundToInt(newValue));
        }
    }
}