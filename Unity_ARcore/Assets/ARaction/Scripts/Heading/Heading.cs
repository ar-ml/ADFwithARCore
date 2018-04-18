using UnityEngine;

namespace ARaction
{
    public abstract class Heading : MonoBehaviour
    {
        public Quaternion? Rotation { get; private set; }

        public void Update()
        {
            Rotation = CalculateRotation();
        }

        protected abstract Quaternion? CalculateRotation();
    }
}