using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
    public int Speed = 5;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            MoveForward();
        }
        if (Input.GetKey(KeyCode.A))
        {
            MoveLeft();
        }
        if (Input.GetKey(KeyCode.S))
        {
            MoveBack();
        }
        if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
        }
        if (Input.GetKey(KeyCode.Q))
        {
            Lrotate();
        }
        if (Input.GetKey(KeyCode.E))
        {
            Rrotate();
        }
    }
    void MoveForward()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * Speed);
    }
    void MoveBack()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * -Speed);
    }
    void MoveLeft()
    {
        transform.Translate(Vector3.left * Time.deltaTime * Speed);
    }
    void MoveRight()
    {
        transform.Translate(Vector3.left * Time.deltaTime * -Speed);
    }
    void Lrotate()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * Speed);
    }
    void Rrotate()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * -Speed);
    }


}
