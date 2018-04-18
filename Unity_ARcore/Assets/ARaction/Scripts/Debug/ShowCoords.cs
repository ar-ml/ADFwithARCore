using UnityEngine;
using UnityEngine.UI;

namespace ARaction
{
    public class ShowCoords : MonoBehaviour
    {
        [SerializeField] private Transform observed = null;
        [SerializeField] private Text observer = null;

        private void Update()
        {
            observer.text = observed.position.ToString() + " " + Mathf.RoundToInt(observed.rotation.eulerAngles.y).ToString("D3");        
        }
    }
}