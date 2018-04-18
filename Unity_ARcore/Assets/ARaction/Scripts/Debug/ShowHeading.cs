using UnityEngine;
using UnityEngine.UI;

namespace ARaction
{
    public class ShowHeading : MonoBehaviour
    {
        [SerializeField] private Heading heading = null;
        [SerializeField] private Text display = null;

        public void Update()
        {
            display.text = heading.ToString();
        }
    }
}