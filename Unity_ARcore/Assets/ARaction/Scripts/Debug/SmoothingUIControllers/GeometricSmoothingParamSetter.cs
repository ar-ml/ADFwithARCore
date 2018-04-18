using UnityEngine;
using UnityEngine.UI;

namespace ARaction
{
    [CreateAssetMenu(menuName = "ARaction/Smoothing/GeometricParamSetter")]
    public class GeometricSmoothingParamSetter : SmoothingParamSetter
    {
        public override Smoothing Smoothing { get { return m_Smoothing; } }

        [SerializeField] private Slider zeroToOneSliderPrefab = null;
        [SerializeField] private GeometricSmoothing smoothingPrefab = null;

        private Slider m_curFrameWeightSlider;
        private GeometricSmoothing m_Smoothing;

        protected override void Init(Transform uiParent)
        {
            m_curFrameWeightSlider = Instantiate<Slider>(zeroToOneSliderPrefab, uiParent.transform);
            m_Smoothing = Instantiate<GeometricSmoothing>(smoothingPrefab);
            m_Smoothing.name = smoothingPrefab.name;
            m_curFrameWeightSlider.value = m_Smoothing.CurFrameWeight;
            m_curFrameWeightSlider.onValueChanged.AddListener(newValue => m_Smoothing.CurFrameWeight = newValue);
        }
    }
}