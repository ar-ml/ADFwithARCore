using UnityEngine;

namespace ARaction
{
    [CreateAssetMenu(menuName = "ARaction/Smoothing/Geometric")]
    public class GeometricSmoothing : Smoothing
    {
        public float CurFrameWeight = 0.5F;
        private float? m_CurValue;

        protected override float CalcLastSmoothed(float newValue)
        {
            m_CurValue = Mathf.Lerp(m_CurValue.GetValueOrDefault(newValue), newValue, CurFrameWeight);
            return m_CurValue.Value;
        }

        public override void Reset()
        {
            m_CurValue = null;
        }
    }
}