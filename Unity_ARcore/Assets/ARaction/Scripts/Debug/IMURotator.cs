using UnityEngine;

namespace ARaction
{
    public class IMURotator : MonoBehaviour
    {
        [SerializeField] private Heading heading = null;
        public Heading Heading { set { heading = value; } }

        public void Update()
        {
            if (heading != null)
            {
                transform.localRotation = heading.Rotation.GetValueOrDefault(transform.localRotation);
            }
        }
    }
}