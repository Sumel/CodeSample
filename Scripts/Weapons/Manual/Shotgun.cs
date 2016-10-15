using UnityEngine;
using System;
namespace AssemblyCSharp
{
    public class Shotgun : ChargableWeapon
    {
        [Range(1, 360)]
        [SerializeField]
        private float radius = 60.0f;

        protected override void Shoot()
        {
            Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
            Vector2 dir = (mousePosition - firePointPosition).normalized;

            for (int i = 0; i < power; i++)
            {
                float angle = UnityEngine.Random.Range(0.0f, radius) - radius / 2.0f;
                Vector2 rotatedVector = Quaternion.AngleAxis(angle, Vector3.forward) * dir;
                RaycastHit2D hit = Physics2D.Raycast(firePointPosition, rotatedVector, range, whatToHit);


                if (hit.collider != null)
                {
                    hit.collider.SendMessage("TakeDamage", new DamageInstance(Damage, GameManager.instance.PlayerGameObject), SendMessageOptions.DontRequireReceiver);
                }

                Vector2 destination = (hit.collider != null) ? hit.centroid : (firePointPosition + rotatedVector * range);
                float angleOfParticle = Vector2.Angle(rotatedVector, Vector2.right);
                if (destination.y < firePointPosition.y)
                {
                    angleOfParticle = -angleOfParticle;
                }
                DrawLineToPoint(destination, Quaternion.Euler(0, 0, angleOfParticle));
            }
        }

        protected void DrawLineToPoint(Vector2 destination, Quaternion rotation)
        {
            Vector2 start = firePoint.position;
            float length = (destination - start).magnitude;

            Transform line = (Transform)Instantiate(BulletTrailPrefab, firePoint.position, rotation);

            Vector3 s = line.localScale;
            s.x = length;
            line.localScale = s;
        }

    }
}

