using System;
using System.Collections;
using UnityEngine;
namespace AssemblyCSharp
{
    public class BasicWeapon : MonoBehaviour
    {
        [SerializeField]
        protected float Damage = 5;
        [SerializeField]
        protected Transform BulletTrailPrefab = null;

        [SerializeField]
        protected LayerMask whatToHit;

        [SerializeField]
        MusicParameters musicParameters;


        [SerializeField]
        protected float range = 100;

        protected MusicManager musicManager;
        protected Transform firePoint;
        protected GameManager gameManager;
        public void Awake()
        {
            firePoint = transform.FindChild("FirePoint");
            musicManager = MusicManager.instance;
            if (firePoint == null)
            {
                Debug.LogError("No firepoint found for weapon");
            }
        }

        protected virtual void Start()
        {
            gameManager = GameManager.instance;
        }

        protected virtual void Shoot()
        {
            Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
            RaycastHit2D hit = Physics2D.Raycast(firePointPosition, (mousePosition - firePointPosition).normalized, range, whatToHit);
            Effect();

            if (hit.collider != null)
            {
                hit.collider.SendMessage("TakeDamage", new DamageInstance(Damage, GameManager.instance.PlayerGameObject), SendMessageOptions.DontRequireReceiver);
            }
        }

        protected void Effect()
        {
            Instantiate(BulletTrailPrefab, firePoint.position, firePoint.rotation);
        }

        private IEnumerator selectOnFirstFrame()
        {
            yield return null;
            OnSelect();
        }


        public virtual void OnSelect()
        {
            musicManager.ChangeMusicParameters(musicParameters);
        }

        public virtual void OnDeselect()
        {

        }
    }
}

