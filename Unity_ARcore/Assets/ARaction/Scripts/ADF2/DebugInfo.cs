using UnityEngine;
using UnityEngine.UI;

namespace ARaction
{
    public class DebugInfo : MonoBehaviour
    {
        public Text description;
        public GameObject info;
        public ADF2Wrapper wrapper;

        private float update = 1;

        private void Update()
        {
            if (wrapper.ADFMode == ADF2Wrapper.Mode.RELOCALISATION)
            {
                info.SetActive(true);
                update += Time.deltaTime;

                if (update > 1)
                {
                    description.text = wrapper.DebugInfo();
                    update = 0;
                }
            }
            else
            {
                info.SetActive(false);
            }
        }
    }
}