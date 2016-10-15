
using System;
using UnityEngine;
namespace AssemblyCSharp
{
    public class BasicShooter : BasicAI, BeatResponder
    {
        [SerializeField]
        private GameObject projectilePrefab = null;
        [SerializeField]
        private float beatInterval = 8.0f;
        protected MusicManager musicManager;

        protected override void Start()
        {
            base.Start();
            musicManager = MusicManager.instance;
            musicManager.AddNewResponder(this, beatInterval);
        }

        protected override void TryFighting()
        {
            if (HasClearLineToPlayer)
                StopMovementGradually();
            else
                MoveHorizontallyTowardsPlayer(parameters.MaxSpeedFighting);
        }

        protected virtual void TryAttacking()
        {
            Attack();
        }

        protected override void Attack()
        {
            GameObject newProjectile = (GameObject)GameObject.Instantiate(projectilePrefab);
            newProjectile.transform.position = this.transform.position;
            BasicEnemyProjectile projectileScript = newProjectile.GetComponent<BasicEnemyProjectile>();
            projectileScript.Direction = DirectionToPlayer;
        }

        public virtual void OnBeat(float timeOfBeat, float beatInterval)
        {
            if (currentState == AIState.Fighting)
            {
                TryAttacking();
            }
        }

        protected override void OnDestroy()
        {
            musicManager.RemoveResponder(this, beatInterval);
        }

    }
}

