using UnityEngine;
using System;
using System.Collections.Generic;


namespace AssemblyCSharp
{
    public class SniperRifle : ChargableWeapon
    {

        [SerializeField]
        protected LayerMask blockingLayer;

        //pierces enemies but not walls
        protected override void Shoot()
        {
            Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
            RaycastHit2D[] hits = Physics2D.RaycastAll(firePointPosition, (mousePosition - firePointPosition).normalized, range, whatToHit);

            List<RaycastHit2D> hitsList = new List<RaycastHit2D>(hits);

            hitsList.Sort((h1, h2) => h1.distance.CompareTo(h2.distance));

            Vector2 hitPoint = (Vector2)firePoint.position + (Vector2)((mousePosition - firePointPosition).normalized * range);

            for (int i = 0; i < hitsList.Count; i++)
            {
                RaycastHit2D hit = hitsList[i];
                if (hit.collider != null)
                {
                    hit.collider.SendMessage("TakeDamage", new DamageInstance(Damage, GameManager.instance.PlayerGameObject), SendMessageOptions.DontRequireReceiver);
                    //we've hit a platform. Bullet shouls stop
                    if (((1 << hit.collider.gameObject.layer) & blockingLayer) != 0)
                    {
                        hitPoint = hit.centroid;
                        break;
                    }
                }
            }

            drawLineToPoint(hitPoint);

        }

        protected override void tryShooting()
        {
            Damage = power;
            base.tryShooting();
        }


        private void drawLineToPoint(Vector2 destination)
        {
            Vector2 start = firePoint.position;
            float length = (destination - start).magnitude;
            Transform line = (Transform)Instantiate(BulletTrailPrefab, firePoint.position, firePoint.rotation);
            Vector3 s = line.localScale;
            s.x = length;
            line.localScale = s;
        }


    }
}

