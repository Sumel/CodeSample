using UnityEngine;
using System.Collections;

public class Trigger : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToInform;
    [SerializeField]
    private bool SendEnter = true;
    [SerializeField]
    private bool SendExit = false;
    [SerializeField]
    private bool DestroyAfterEnter = true;
    [SerializeField]
    private bool DestroyAfterExit = true;
    bool isCollidingWithPlayer = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (SendEnter)
        {
            if (!isCollidingWithPlayer && other.tag == "Player")
            {
                isCollidingWithPlayer = true;
                objectToInform.SendMessage("OnPlayerEnterTrigger", other.transform);
                if (DestroyAfterEnter)
                {
                    Destroy(this);
                }
            }
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (SendExit)
        {
            if (isCollidingWithPlayer && other.tag == "Player")
            {
                isCollidingWithPlayer = false;
                objectToInform.SendMessage("OnPlayerExitTrigger", other.transform);
                if (DestroyAfterExit)
                {
                    Destroy(this);
                }
            }
        }

    }
}
