using System;
using UnityEngine;
namespace AssemblyCSharp
{
    public class AutoWeapon : BasicWeapon, MelodyResponder
    {
        MelodyTracker melodyTracker;
        private bool wantsToFire = false;


        protected override void Start()
        {
            base.Start();
            melodyTracker = MusicManager.instance.melodyTracker;
            melodyTracker.Add(this);
        }

        void Update()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                wantsToFire = true;
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                wantsToFire = false;
            }
        }

        public void OnMelody(Note note)
        {
            if (wantsToFire)
            {
                Shoot();
            }
        }
    }
}

