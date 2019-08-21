using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
    public float Speed = 100;
    // Update is called once per frame
    void Update () {
        if (Input.GetKey("down"))
        {
            transform.Rotate(Vector3.right * Time.deltaTime * 50);
        }
        else if (Input.GetKey("up"))
        {
            transform.Rotate(Vector3.left * Time.deltaTime * 50);
        }
        if (Input.GetKey("right"))
        {
            transform.Rotate(Vector3.back * Time.deltaTime * 50);
        }
        else if (Input.GetKey("left"))
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * 50);
        }
    }
}
