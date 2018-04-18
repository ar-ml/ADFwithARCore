using UnityEngine;
using UnityEngine.UI;

namespace ARaction
{
    public class ShowARCoreStatus : MonoBehaviour
    {
        [SerializeField] private Text label;
        [SerializeField] private SceneController sceneController;

        private void Update()
        {
            label.text = sceneController.StatusMessage;
        }
    }
}