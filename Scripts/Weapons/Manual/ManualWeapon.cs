using System;
using UnityEngine;
namespace AssemblyCSharp
{
    public class ManualWeapon : BasicWeapon
    {
        protected MelodyTracker melodyTracker;

        protected HUDManager hudManager;
        protected Note _closestNote;
        protected Note ClosestNote
        {
            get { return _closestNote; }
            set { _closestNote = value; }
        }
        protected MelodyModule melodyModule;
        protected override void Start()
        {
            base.Start();
            musicManager = MusicManager.instance;
            melodyTracker = musicManager.melodyTracker;
            hudManager = HUDManager.instance;
            melodyModule = new MelodyModule();
        }

        protected virtual void Update()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                tryShooting();
            }
        }

        protected virtual void tryShooting()
        {
            RhythmAccuracy acc = melodyModule.Activate();
            switch (acc)
            {
                case RhythmAccuracy.Full:
                    hudManager.ShotRating.SendMessage("CheckAccuracyDisplay", RhythmAccuracy.Full);
                    Shoot();
                    break;
                case RhythmAccuracy.Partial:
                    hudManager.ShotRating.SendMessage("CheckAccuracyDisplay", RhythmAccuracy.Partial);
                    Shoot();
                    break;
                case RhythmAccuracy.Miss:
                    hudManager.ShotRating.SendMessage("CheckAccuracyDisplay", RhythmAccuracy.Miss);
                    break;
            }
        }
    }
}

