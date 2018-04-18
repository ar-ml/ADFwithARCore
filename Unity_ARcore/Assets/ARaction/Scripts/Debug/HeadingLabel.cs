using UnityEngine;
using UnityEngine.UI;

namespace ARaction
{
    public class HeadingLabel : MonoBehaviour
    {
        [SerializeField] private Text headingLabel = null;

        public void OnHeadingStrategyChanged(ARCoreHeadingOffset headingStrategy)
        {
            if (headingStrategy != null)
            {
                headingLabel.text = headingStrategy.name;
            }
        }
    }
}