using System.Collections.Generic;
using UnityEngine;

namespace ARaction
{
    public class RotatorParamChooser : MonoBehaviour
    {
        [SerializeField] private ARCoreHeadingOffset[] headingOffsetPrefabs = null;
        [SerializeField] private RotatorParamSet paramSetPrefab = null;
        [SerializeField] private HeadingOffsetEvent onHeadingStrategyChanged = null;
        [SerializeField] private CanvasRenderer smootherUIParent = null;

        private RotatorParamSet CurParamSet { get { return m_ParamSets[m_CurParamSetIdx]; } }
        private int m_CurParamSetIdx;
        private List<RotatorParamSet> m_ParamSets;

        public void Start()
        {
            CreateParamSets();
            ActivateCurParamSet();
        }

        public void OnNextParamSet()
        {
            CurParamSet.IsUIVisible = false;
            m_CurParamSetIdx = (m_CurParamSetIdx + 1) % m_ParamSets.Count;
            ActivateCurParamSet();
        }

        public void OnNextSmoothing()
        {
            CurParamSet.NextSmoothing();
        }

        private void ActivateCurParamSet()
        {
            CurParamSet.IsUIVisible = true;
            RaiseOnHeadingStrategyChanged();
        }

        private void RaiseOnHeadingStrategyChanged()
        {
            if (onHeadingStrategyChanged != null)
            {
                onHeadingStrategyChanged.Invoke(CurParamSet.HeadingOffset);
            }
        }

        private void CreateParamSets()
        {
            m_ParamSets = new List<RotatorParamSet>(headingOffsetPrefabs.Length);
            foreach (var prefab in headingOffsetPrefabs)
            {
                RotatorParamSet paramSet = Instantiate<RotatorParamSet>(paramSetPrefab);
                paramSet.HeadingOffset = Instantiate<ARCoreHeadingOffset>(prefab, transform);
                paramSet.HeadingOffset.name = prefab.name;
                paramSet.Init(smootherUIParent);
                paramSet.IsUIVisible = false;
                m_ParamSets.Add(paramSet);
            }
        }
    }
}