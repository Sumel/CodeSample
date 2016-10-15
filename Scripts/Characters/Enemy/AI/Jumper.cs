
using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Jumper : BasicAI, BeatResponder
    {
        MusicManager musicManager;
        private bool justJumped = false;
        protected override void Start()
        {
            base.Start();
            musicManager = MusicManager.instance;
            musicManager.AddNewResponder(this, 1.0f);
        }

        public override void Update()
        {
            base.Update();
            if (Controller.State.IsGrounded && !justJumped)
            {
                Controller.SetHorizontalForce(0);

            }
            if (justJumped)
            {
                justJumped = false;
            }
        }

        protected override void TryChasing() { }
        protected override void TryFighting() { }

        public void OnBeat(float timeOfBeat, float a)
        {
            if (currentState != AIState.Idle && Controller.State.IsGrounded)
            {
                float speed = 0;
                if (currentState == AIState.Chasing)
                {
                    speed = parameters.MaxSpeedChasing;
                }
                else
                {
                    speed = parameters.MaxSpeedFighting;
                }
                float normalizedHorizontalSpeed = (player.transform.position.x > transform.position.x) ? 1 : -1;
                Controller.SetHorizontalForce(speed * normalizedHorizontalSpeed);
                Controller.Jump();
                justJumped = true;
            }
        }

        protected override void OnDestroy()
        {
            if (musicManager != null)
                musicManager.RemoveResponder(this, 1.0f);
        }
    }
}

