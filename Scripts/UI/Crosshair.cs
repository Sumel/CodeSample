using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
        Vector2 mousePosition = new Vector2 (Camera.main.ScreenToWorldPoint(Input.mousePosition).x,Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        transform.position = mousePosition;
	}
}
