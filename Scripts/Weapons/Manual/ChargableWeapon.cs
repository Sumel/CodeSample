
using System;
using UnityEngine;


namespace AssemblyCSharp
{
    public class ChargableWeapon : ManualWeapon
    {
        [SerializeField]
        protected float minPower = 10.0f;

        [SerializeField]
        protected float maxPower = 100.0f;

        protected RhythmAccuracyModule powerShotModule;
        protected RhythmAccuracyModule chargeModule;

        protected float power = 1.0f;

        [SerializeField]
        protected float fullCharge = 5.0f;
        [SerializeField]
        protected float partialCharge = 5.0f;
        [SerializeField]
        protected float missChargeMultiplier = 0.75f;
        [SerializeField]
        protected float missChargeSubtractor = 0.0f;
        [SerializeField]
        protected float shootingCost = 10.0f;
        [SerializeField]
        protected float missShootingCost = 5.0f;

        protected override void Start()
        {
            base.Start();
            powerShotModule = new BeatModule(1.0f, gameManager.PartialThreshold, gameManager.PerfectThreshold);
            chargeModule = new BeatModule(1.0f, gameManager.PartialThreshold, gameManager.PerfectThreshold);

            power = minPower;
            UpdateHudAmmo();
        }

        protected override void Update()
        {

            if (Input.GetButtonDown("Fire2"))
            {
                tryCharging();
                CheckPower();
                UpdateHudAmmo();
            }
            if (Input.GetButtonDown("Fire1"))
            {
                tryShooting();
                CheckPower();
                UpdateHudAmmo();
            }
        }

        protected override void tryShooting()
        {
            RhythmAccuracy acc = powerShotModule.Activate();

            switch (acc)
            {
                case RhythmAccuracy.Full:
                    hudManager.ShotRating.SendMessage("CheckAccuracyDisplay", RhythmAccuracy.Full);
                    Shoot();
                    power -= shootingCost;
                    break;
                case RhythmAccuracy.Partial:
                    hudManager.ShotRating.SendMessage("CheckAccuracyDisplay", RhythmAccuracy.Partial);
                    Shoot();
                    power -= shootingCost;
                    break;
                case RhythmAccuracy.Miss:
                    power -= missShootingCost;
                    hudManager.ShotRating.SendMessage("CheckAccuracyDisplay", RhythmAccuracy.Miss);
                    break;
            }

        }

        protected virtual void tryCharging()
        {
            RhythmAccuracy acc = chargeModule.Activate();
            switch (acc)
            {
                case RhythmAccuracy.Full:
                    hudManager.ChargeRating.SendMessage("CheckAccuracyDisplay", RhythmAccuracy.Full);
                    power += fullCharge;
                    break;
                case RhythmAccuracy.Partial:
                    hudManager.ChargeRating.SendMessage("CheckAccuracyDisplay", RhythmAccuracy.Partial);
                    power += partialCharge;
                    break;
                case RhythmAccuracy.Miss:
                    hudManager.ChargeRating.SendMessage("CheckAccuracyDisplay", RhythmAccuracy.Miss);
                    power *= missChargeMultiplier;
                    power -= missChargeSubtractor;
                    break;
            }
        }

        protected void CheckPower()
        {
            if (power < minPower)
            {
                power = minPower;
            }
            else if (power > maxPower)
            {
                power = maxPower;
            }

        }

        public override void OnSelect()
        {
            base.OnSelect();
            UpdateHudAmmo();
        }

        protected void UpdateHudAmmo()
        {
            hudManager.AmmoDisplay.SendMessage("SetDisplayedAmmo", power);
        }
    }
}

