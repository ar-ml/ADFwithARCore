using GoogleARCore;
using System.Collections;
using UnityEngine;

namespace ARaction
{
    public class SceneController : MonoBehaviour
    {
        private string m_statusMessage;

        public string StatusMessage
        {
            get { return m_statusMessage; }
            private set
            {
                if (!m_IsQuitting)
                {
                    m_statusMessage = value;
                }
            }
        }

        private bool m_IsQuitting;

        public void Start()
        {
            QuitOnConnectionErrors();
        }

        public void Update()
        {
            QuitOnConnectionErrors();

            StatusMessage = Session.Status.ToString();

            // The session status must be Tracking in order to access the Frame.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;
                return;
            }
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void QuitOnConnectionErrors()
        {
            if (m_IsQuitting)
            {
                return;
            }
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                StartCoroutine(ExitWithError("Camera permission is needed to run this application."));
            }
            else if (Session.Status.IsError())
            {
                // This covers a variety of errors.  See reference for details
                // https://developers.google.com/ar/reference/unity/namespace/GoogleARCore
                StartCoroutine(ExitWithError("ARCore encountered a problem connecting. Please restart the app."));
            }
        }

        private IEnumerator ExitWithError(string msg, int waitBeforeQuitting = 5)
        {
            StatusMessage = msg;
            m_IsQuitting = true;
            yield return new WaitForSeconds(waitBeforeQuitting);
            Application.Quit();
        }
    }
}