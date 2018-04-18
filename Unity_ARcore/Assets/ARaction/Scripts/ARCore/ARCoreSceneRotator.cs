using UnityEngine;

namespace ARaction
{
    public class ARCoreSceneRotator : MonoBehaviour
    {
        [SerializeField] private ARCoreHeadingOffset offset = null;

        public void Update()
        {
            transform.localRotation = offset.HeadingOffsetFromPose;
        }
    }
}