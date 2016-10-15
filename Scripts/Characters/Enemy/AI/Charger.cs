
using System;
using UnityEngine;
namespace AssemblyCSharp
{
    public class Charger : BasicAI, BeatResponder
    {
        [SerializeField]
        float speedOnBeatMultiplier = 2;
        MusicManager musicManager;

        [SerializeField]
        float minimalDistanceToChangeDirection = 2;

        private bool isCharging = false;

        protected override void Start()
        {
            base.Start();
            musicManager = MusicManager.instance;
            musicManager.AddNewResponder(this, 1.0f);
        }

        protected override void TryFighting()
        {
            if (isCharging)
                MoveInLastDirection(parameters.MaxSpeedFighting);
            else
                MoveInLastDirection(parameters.MaxSpeedFighting);
        }

        public void OnBeat(float timeOfBeat, float a)
        {
            ActivateCharge(musicManager.BeatInterval / 5.0f);
        }

        private void ActivateCharge(float timeOfCharge)
        {
            parameters.MaxSpeedFighting *= speedOnBeatMultiplier;
            parameters.MaxSpeedChasing *= speedOnBeatMultiplier;
            isCharging = true;
            if (player != null && DistanceToPlayer > minimalDistanceToChangeDirection)
            {
                turnToPlayer();
            }

            GameManager.DelayedFunction(timeOfCharge, ReturnToNormal);
        }

        private void ReturnToNormal()
        {
            if (isCharging)
            {
                parameters.MaxSpeedFighting /= speedOnBeatMultiplier;
                parameters.MaxSpeedChasing /= speedOnBeatMultiplier;
                isCharging = false;
            }
        }

        protected override void OnDestroy()
        {
            if (musicManager != null)
            {
                musicManager.RemoveResponder(this, 1.0f);
            }

        }
    }
}

