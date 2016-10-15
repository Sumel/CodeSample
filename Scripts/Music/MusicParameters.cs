
using System;
using UnityEngine;


namespace AssemblyCSharp
{
    [Serializable]
    public class MusicParameters
    {
        public float BPM;
        public float Offset;
        public TextAsset Beatmap = null;
        public AudioClip Song;
    }
}

