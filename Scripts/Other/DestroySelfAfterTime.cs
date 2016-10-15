
using System;
using UnityEngine;


namespace AssemblyCSharp
{
    public class DestroySelfAfterTime : MonoBehaviour
    {
        [Range(0.0f, 1000.0f)]
        [SerializeField]
        float lifeTime = 0.0f;

        void Start()
        {
            Destroy(gameObject, lifeTime);
        }


    }
}

