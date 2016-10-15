
using System;
using UnityEngine;


namespace AssemblyCSharp
{
    public class ObjectActivator : MonoBehaviour, TriggerEnterResponder
    {
        [SerializeField]
        private GameObject[] objectsToActivate = null;

        void Start()
        {
            foreach (GameObject ob in objectsToActivate)
            {
                ob.SetActive(false);
            }
        }

        public void OnPlayerEnterTrigger(Transform p)
        {
            foreach (GameObject ob in objectsToActivate)
            {
                ob.SetActive(true);
            }
        }

    }
}

