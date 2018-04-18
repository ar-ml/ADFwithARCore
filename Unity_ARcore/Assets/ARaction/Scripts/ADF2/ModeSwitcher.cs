using UnityEngine;
using UnityEngine.UI;

namespace ARaction
{
    public class ModeSwitcher : MonoBehaviour
    {
        public Text description;
        public ADF2Wrapper wrapper;

        public void SwitchMode()
        {
            switch (wrapper.ADFMode)
            {
                case ADF2Wrapper.Mode.DUMMY:
                    wrapper.ADFMode = ADF2Wrapper.Mode.RELOCALISATION;
                    break;

                case ADF2Wrapper.Mode.LEARNING:
                    wrapper.ADFMode = ADF2Wrapper.Mode.DUMMY;
                    break;

                case ADF2Wrapper.Mode.RELOCALISATION:
                    wrapper.ADFMode = ADF2Wrapper.Mode.LEARNING;
                    break;
            }
        }

        public void Update()
        {
            switch (wrapper.ADFMode)
            {
                case ADF2Wrapper.Mode.DUMMY:
                    description.text = "Without relocal.";
                    break;

                case ADF2Wrapper.Mode.LEARNING:
                    description.text = "Area learning";
                    break;

                case ADF2Wrapper.Mode.RELOCALISATION:
                    description.text = "Relocalisation";
                    break;
            }
        }
    }
}