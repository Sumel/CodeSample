using System;
using UnityEngine;
namespace AssemblyCSharp
{
    public class MelodyTicker : MonoBehaviour, MelodyResponder
    {
        MelodyTracker melodyTracker;
        void Start()
        {
            melodyTracker = MusicManager.instance.melodyTracker;
            melodyTracker.Add(this);
        }
        public void OnMelody(Note note)
        {
            GetComponent<AudioSource>().Play();
        }
    }
}

