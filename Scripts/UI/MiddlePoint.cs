using UnityEngine;
using System.Collections;

public class MiddlePoint : MonoBehaviour
{

    [SerializeField]
    Transform crosshair;
    [SerializeField]
    Transform player;

    void Update()
    {
        Vector2 pos = (crosshair.position + player.position + player.position) / 3.0f;
        transform.position = pos;
    }
}
