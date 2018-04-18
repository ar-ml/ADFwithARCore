using System.Collections.Generic;
using UnityEngine;

namespace ARaction
{
    [CreateAssetMenu(menuName = "ARaction/RotatorParamSet")]
    public class RotatorParamSet : ScriptableObject
    {
        public ARCoreHeadingOffset HeadingOffset { get; set; }
        public bool IsUIVisible { set { m_SmoothingParamSetters[m_CurSmoothingIdx].Visible = value; } }

        [SerializeField] private SmoothingParamSetter[] smoothingParamSetterPrefabs = null;

        private int m_CurSmoothingIdx;

        private List<SmoothingParamSetter> m_SmoothingParamSetters;

        public void Init(CanvasRenderer smootherUIParent)
        {
            m_SmoothingParamSetters = CreateSmoothingParamSetters(smoothingParamSetterPrefabs, smootherUIParent);
            UpdateHeadingOffsetSmoothing();
        }

        public void NextSmoothing()
        {
            IsUIVisible = false;
            m_CurSmoothingIdx = (m_CurSmoothingIdx + 1) % m_SmoothingParamSetters.Count;
            UpdateHeadingOffsetSmoothing();
            IsUIVisible = true;
        }

        private void UpdateHeadingOffsetSmoothing()
        {
            HeadingOffset.Smoothing = m_SmoothingParamSetters[m_CurSmoothingIdx].Smoothing;
        }

        private List<SmoothingParamSetter> CreateSmoothingParamSetters(SmoothingParamSetter[] prefabs, CanvasRenderer uiParent)
        {
            var smoothingParamSetters = new List<SmoothingParamSetter>(prefabs.Length);

            foreach (var prefab in prefabs)
            {
                var smoothingParamSetter = Instantiate<SmoothingParamSetter>(prefab);
                smoothingParamSetter.Init(uiParent);
                smoothingParamSetter.Visible = false;
                smoothingParamSetters.Add(smoothingParamSetter);
            }
            return smoothingParamSetters;
        }
    }
}