using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine.UI;

public class move : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
   
    public double pitch;
    public double roll;
    public double yaw;
   
   

    // Update is called once per frame
    void Update () {
        if (Input.GetKey(KeyCode.UpArrow)){
            transform.Rotate(Vector3.right * Time.deltaTime * 30);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Rotate(Vector3.left * Time.deltaTime * 30);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * 30);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.back * Time.deltaTime * 30);
        }
        if (Input.GetKey(KeyCode.RightControl))
        {
            transform.Rotate(Vector3.down * Time.deltaTime * 30);
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 30);
        }


       

    }
}
