using UnityEngine;
using System;
namespace AssemblyCSharp
{
    public class DestroySelfOnBeat : MonoBehaviour, BeatResponder
    {
        [Range(1, 16)]
        [SerializeField]
        int waitBeats = 1;
        int counter = 0;


        MusicManager musicManager;
        void Start()
        {
            musicManager = MusicManager.instance;
            musicManager.AddNewResponder(this, 1.0f);
        }

        public void OnBeat(float a, float b)
        {
            counter++;
            if (counter >= waitBeats)
            {
                musicManager.RemoveResponder(this, 1.0f);
                Destroy(gameObject);
            }
        }


    }
}

