using System;
using UnityEngine;
using UnityEngine.UI;

namespace ARaction
{
    public class DriftInfo : MonoBehaviour
    {
        public Text description;
        public ADF2Wrapper wrapper;

        private float update = 1;

        private void Update()
        {
            update += Time.deltaTime;
            if (update > 1)
            {
                float drift = wrapper.EstimateDrift();
                if (drift > 999)
                {
                    description.text = "Tracking lost";
                }
                else
                {
                    description.text = "Drift: " + Math.Round(drift, 2);
                }
                update = 0;
            }
        }
    }
}