using UnityEngine;

namespace ARaction
{
    public class GyroscopeAttitudeHeading : Heading
    {
        protected readonly Quaternion m_BaseOrientation = Quaternion.Euler(-90, 0, 0); // make "forward" the back of the phone instead of the top

        public override string ToString()
        {
            return "Gyro: " + ((SystemInfo.supportsGyroscope) ? Rotation.Value.ToString("F2") : "???");
        }

        public void OnEnable()
        {
            if (SystemInfo.supportsGyroscope)
            {
                EnableSensors();
            }
            else
            {
                Debug.LogWarning("Gyroscope not supported");
            }
        }

        protected override Quaternion? CalculateRotation()
        {
            if (SystemInfo.supportsGyroscope)
            {
                return FlipHandedness(m_BaseOrientation * Input.gyro.attitude);
            }

            return null;
        }

        // Gyroscope is right-handed, Unity is left-handed
        protected Quaternion FlipHandedness(Quaternion q)
        {
            return new Quaternion(q.x, q.y, -q.z, -q.w);
        }

        private static void EnableSensors()
        {
            Input.gyro.enabled = true;
            Input.compass.enabled = true;
        }
    }
}