using UnityEngine;

namespace ARaction
{
    public class RotateWithSlider : MonoBehaviour
    {
        public void Rotate(float yRot)
        {
            transform.rotation = Quaternion.Euler(0, yRot, 0);
        }
    }
}