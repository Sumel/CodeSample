using UnityEngine;
using System;
namespace AssemblyCSharp
{
    public class FakeSongStarter : MonoBehaviour
    {
        [SerializeField]
        MusicParameters musicParameters;

        void Start()
        {
            GameManager.DelayedFunction(0,InitSong);
        }

        private void InitSong()
        {
            MusicManager manager = MusicManager.instance;
            manager.ChangeMusicParameters(musicParameters);
        }
    }
}

