using UnityEngine;
using UnityEngine.UI;

namespace ARaction
{
    public class ShowToString : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour observed = null;
        [SerializeField] private Text observer = null;

        private void Update()
        {
            observer.text = observed.ToString();
        }
    }
}