using System.Collections.Generic;
using UnityEngine;

namespace ARaction
{
    [CreateAssetMenu(menuName = "ARaction/Smoothing/MovingWindow")]
    public class MovingWindowSmoothing : Smoothing
    {
        public int WindowSize = 5;
        private Queue<float> m_PastValues = new Queue<float>();
        private float m_RunningTotal;

        protected override float CalcLastSmoothed(float newValue)
        {
            m_RunningTotal += newValue;
            while (m_PastValues.Count >= WindowSize)
            {
                m_RunningTotal -= m_PastValues.Dequeue();
            }
            m_PastValues.Enqueue(newValue);
            return m_RunningTotal / m_PastValues.Count;
        }

        public override void Reset()
        {
            m_PastValues.Clear();
            m_RunningTotal = 0;
        }

        public override string ToString()
        {
            return "total = " + Mathf.RoundToInt( m_RunningTotal ).ToString("D3") + " count " + m_PastValues.Count;
        }

        public void OnChangeWindowSize(float newWindowSize)
        {
            WindowSize = Mathf.RoundToInt(newWindowSize);
        }
    }
}