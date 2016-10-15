using System;
using UnityEngine;


namespace AssemblyCSharp
{
    public class DamageTrigger : MonoBehaviour
    {
        bool isCollidingWithPlayer = false;
        void OnTriggerEnter2D(Collider2D other)
        {
            if (!isCollidingWithPlayer && other.tag == "Player")
            {
                isCollidingWithPlayer = true;
                transform.parent.SendMessage("OnPlayerEnterDamage", other.transform);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (isCollidingWithPlayer && other.tag == "Player")
            {
                isCollidingWithPlayer = false;
                transform.parent.SendMessage("OnPlayerExitDamage", other.transform);
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                transform.parent.SendMessage("OnPlayerStayDamage", other.transform);
            }
        }
    }
}

