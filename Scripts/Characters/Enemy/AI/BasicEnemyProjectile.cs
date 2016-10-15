using UnityEngine;
using System;
namespace AssemblyCSharp
{
    //TODO: pooling instead of spawning new projectiles

    public class BasicEnemyProjectile : MonoBehaviour
    {
        [SerializeField]
        protected float speed = 10;
        [SerializeField]
        protected float damage = 1;
        [SerializeField]
        protected float lifeTime = 10;
        [SerializeField]
        private LayerMask layerToIgnore;


        protected float destroyTime = 0;

        private Vector2 _direction;

        protected BoxCollider2D _collider;

        private GameObject _owner;

        public GameObject Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public Vector2 Direction
        {
            get { return _direction; }
            set { _direction = value.normalized; }
        }

        public void Awake()
        {
            destroyTime = Time.realtimeSinceStartup + lifeTime;
            _collider = GetComponent<BoxCollider2D>();
        }

        public void Update()
        {
            if (Time.realtimeSinceStartup > destroyTime)
            {
                TryDestroy();
                return;
            }
            Move();
        }

        protected virtual void Move()
        {
            Vector2 deltaDir = Direction * speed * Time.deltaTime;
            transform.position += (Vector3)deltaDir;
        }

        protected virtual void onPlayerHit(GameObject player)
        {
            player.SendMessage("TakeDamage", new DamageInstance(damage, gameObject, Owner));
        }

        public virtual void OnCollisionEnter2D(Collision2D coll)
        {
            Collider2D other = coll.collider;
            if (other.tag == "Player")
            {
                onPlayerHit(other.gameObject);
                TryDestroy();
            }
            else if (coll.collider.gameObject.layer != layerToIgnore)
            {
                TryDestroy();
            }
        }

        protected void TryDestroy()
        {
            Destroy(gameObject);
        }

    }
}

