using UnityEngine;
using System.Collections;

public class FightingTrigger : MonoBehaviour
{
    bool isCollidingWithPlayer = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCollidingWithPlayer && other.tag == "Player")
        {
            isCollidingWithPlayer = true;
            transform.parent.SendMessage("OnPlayerEnterFighting", other.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (isCollidingWithPlayer && other.tag == "Player")
        {
            isCollidingWithPlayer = false;
            transform.parent.SendMessage("OnPlayerExitFighting", other.transform);
        }
    }
}
