using GoogleARCore;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ARaction
{
    /// <summary>
    /// Controls the HelloAR example.
    /// </summary>
    public class HelloARactionController : MonoBehaviour
    {
        [SerializeField] private ARCoreHeadingOffset offset = null;

        public GameObject ARSession;

        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a plane.
        /// </summary>
        public GameObject AndyAndroidPrefab;

        private List<GameObject> m_andys = new List<GameObject>();

        private Vector3 lastOffset = Vector3.zero;

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;

        public void AndysRemove()
        {
            foreach (GameObject andy in m_andys)
            {
                Destroy(andy);
            }
            m_andys.Clear();
        }

        public void AndysRestore()
        {
            AndysRemove();
            string filename = Application.persistentDataPath + "/andys.txt";
            if (File.Exists(filename))
            {
                string[] data = File.ReadAllLines(filename);
                foreach (string s in data)
                {
                    string[] values = s.Split(' ');
                    GameObject andy = Instantiate(AndyAndroidPrefab);
                    Vector3 position;
                    position.x = float.Parse(values[0]);
                    position.y = float.Parse(values[1]);
                    position.z = float.Parse(values[2]);
                    andy.transform.position = position + ARSession.transform.position;
                    Quaternion rotation;
                    rotation.x = float.Parse(values[3]);
                    rotation.y = float.Parse(values[4]);
                    rotation.z = float.Parse(values[5]);
                    rotation.w = float.Parse(values[6]);
                    andy.transform.rotation = rotation;
                    m_andys.Add(andy);
                }
            }
        }

        public void AndysSave()
        {
            string output = "";
            foreach (GameObject andy in m_andys)
            {
                output += andy.transform.position.x + " ";
                output += andy.transform.position.y + " ";
                output += andy.transform.position.z + " ";
                output += andy.transform.rotation.x + " ";
                output += andy.transform.rotation.y + " ";
                output += andy.transform.rotation.z + " ";
                output += andy.transform.rotation.w + "\n";
            }
            File.WriteAllText(Application.persistentDataPath + "/andys.txt", output);
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            if (ARSession != null)
            {
                if (lastOffset != ARSession.transform.position)
                {
                    AndysRestore();
                    lastOffset = ARSession.transform.position;
                }
            }

            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            _QuitOnConnectionErrors();

            // Check that motion tracking is tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;

                return;
            }

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // If the player has not touched the screen, we are done with this update.
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                TrackableHitFlags.FeaturePointWithSurfaceNormal;

            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
            {
                Vector3 hitPosition = offset.HeadingOffsetFromPose * hit.Pose.position;
                Quaternion hitRotation = offset.HeadingOffsetFromPose * hit.Pose.rotation;
                var andyObject = Instantiate(AndyAndroidPrefab, hitPosition, hitRotation);

                // Andy should look at the camera but still be flush with the plane.
                if ((hit.Flags & TrackableHitFlags.PlaneWithinPolygon) != TrackableHitFlags.None)
                {
                    // Get the camera position and match the y-component with the hit position.
                    Vector3 cameraPositionSameY = FirstPersonCamera.transform.position;
                    cameraPositionSameY.y = hitPosition.y;

                    // Have Andy look toward the camera respecting his "up" perspective, which may be from ceiling.
                    andyObject.transform.LookAt(cameraPositionSameY, andyObject.transform.up);
                }

                m_andys.Add(andyObject);
                AndysSave();
            }
        }

        /// <summary>
        /// Quit the application if there was a connection error for the ARCore session.
        /// </summary>
        private void _QuitOnConnectionErrors()
        {
            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void _DoQuit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}