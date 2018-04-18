using UnityEngine;

namespace ARaction
{
    public class CompassHeading : Heading
    {
        [SerializeField] private float acceptableInaccuracyInDegrees = 15F;

        protected override Quaternion? CalculateRotation()
        {
            if (IsCompassCalibrated())
            {
                return Quaternion.Euler(0, Input.compass.magneticHeading, 0);
            }

            return null;
        }

        public override string ToString()
        {
            string heading = "Compass: ";
            if (IsCompassCalibrated())

            {
                heading += Input.compass.magneticHeading.ToString("F0");
                if (Input.compass.headingAccuracy > 0)
                {
                    heading += " +/- " + Input.compass.headingAccuracy;
                }
            }
            else
            {
                heading += "???";
            }
            return heading;
        }

        public void OnEnable()
        {
            Input.compass.enabled = true;
        }

        private bool IsCompassCalibrated()
        {
            return Input.compass.headingAccuracy >= 0 && Input.compass.headingAccuracy < acceptableInaccuracyInDegrees;
        }
    }
}