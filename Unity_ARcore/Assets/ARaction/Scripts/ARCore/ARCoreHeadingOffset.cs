using GoogleARCore;
using UnityEngine;

namespace ARaction
{
    public class ARCoreHeadingOffset : MonoBehaviour
    {
        [SerializeField] private Heading heading = null;
        [SerializeField] private Smoothing smoothing = null;

        public Heading Heading { set { heading = value; } }

        public Smoothing Smoothing
        {
            private get
            {
                return smoothing;
            }
            set
            {
                smoothing = value;
                if (smoothing != null)
                {
                    smoothing.Reset();
                }
            }
        }

        private bool m_IsDirty = true;
        private Quaternion m_HeadingOffsetFromPose;

        public Quaternion HeadingOffsetFromPose
        {
            get
            {
                if (m_IsDirty)
                {
                    float smoothedOffset = Smoothing.GetSmoothed(CalcHeadingOffsetFromPose());
                    m_HeadingOffsetFromPose = Quaternion.Euler(0, smoothedOffset, 0);
                    m_IsDirty = false;
                }
                return m_HeadingOffsetFromPose;
            }
        }

        public void Update()
        {
            m_IsDirty = true;
        }

        public override string ToString()
        {
            float rawY = CalcHeadingOffsetFromPose();
            return "thisframe = " + Mathf.RoundToInt(rawY).ToString("D3") + " smoothed " + Mathf.RoundToInt(Smoothing.LastSmoothed).ToString("D3") + "\nsmoothing " + Smoothing.ToString();
        }

        private float CalcHeadingOffsetFromPose()
        {
            if (heading == null || !heading.Rotation.HasValue)
            {
                return 0;
            }
            float offset = heading.Rotation.Value.eulerAngles.y - Frame.Pose.rotation.eulerAngles.y;
            while (offset < 0)
            {
                offset += 360;

            }
            return offset;
        }
    }
}