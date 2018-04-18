using UnityEngine;
using UnityEngine.UI;

namespace ARaction
{
    public class DatasetReset : MonoBehaviour
    {
        public Text description;
        public ADF2Wrapper wrapper;

        private float update = 1;

        public void Reset()
        {
            wrapper.DatasetReset();
        }

        private void Update()
        {
            update += Time.deltaTime;
            if (update > 1)
            {
                description.text = "Reset dataset with " + wrapper.DatasetSize() + " frames";
                update = 0;
            }
        }
    }
}