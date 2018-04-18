using System.Collections;
using UnityEngine;

namespace ARaction
{
    public class DisableAfter : MonoBehaviour
    {
        [SerializeField] private GameObject[] objectsToDisable;
        [SerializeField] private MonoBehaviour[] behavioursToDisable;
        [SerializeField] private float secondsBeforeDisable;

        public void Start()
        {
            StartCoroutine(DelayedDisable());
        }

        private IEnumerator DelayedDisable()
        {
            yield return new WaitForSeconds(secondsBeforeDisable);
            foreach (GameObject o in objectsToDisable)
            {
                o.SetActive(false);
            }
            foreach (MonoBehaviour b in behavioursToDisable)
            {
                b.enabled = false;
            }
        }
    }
}