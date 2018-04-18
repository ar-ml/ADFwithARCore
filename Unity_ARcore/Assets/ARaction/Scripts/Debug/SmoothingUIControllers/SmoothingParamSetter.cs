using UnityEngine;

namespace ARaction
{
    public abstract class SmoothingParamSetter : ScriptableObject
    {
        public abstract Smoothing Smoothing { get; }
        public bool Visible { set { m_ui.SetActive(value); } }
        [SerializeField] private GameObject uiPrefab = null;
        private GameObject m_ui;

        public void Init(CanvasRenderer parent)
        {
            m_ui = Instantiate<GameObject>(uiPrefab, parent.transform);
            Init(m_ui.transform);
        }

        protected abstract void Init(Transform uiParent);
    }
}