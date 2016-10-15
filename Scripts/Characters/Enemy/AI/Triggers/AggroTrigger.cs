using UnityEngine;
using System.Collections;

public class AggroTrigger : MonoBehaviour
{
    bool isCollidingWithPlayer = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCollidingWithPlayer && other.tag == "Player")
        {
            isCollidingWithPlayer = true;
            transform.parent.SendMessage("OnPlayerEnterAggro", other.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (isCollidingWithPlayer && other.tag == "Player")
        {
            isCollidingWithPlayer = false;
            transform.parent.SendMessage("OnPlayerExitAggro", other.transform);
        }
    }
}
