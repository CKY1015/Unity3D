using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vrfollow : MonoBehaviour {

    public Rigidbody drone;
    private bool rotation_flag = false;

    // Use this for initialization
    void Start () {
        
    }

    // Update is called once per frame
    void Update () {
        Vector3 offset = new Vector3(10, 6, -16);
 
        transform.position = drone.transform.position + offset;
        //Debug.Log(new Vector3(0, transform.localEulerAngles.y, 0));
        if (rotation_flag)
        {
            transform.rotation = drone.transform.rotation;
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, drone.transform.localEulerAngles.y, 0));
        }
        if (Input.GetKey("1"))
        {
            rotation_flag = true;
            Debug.Log("switch");
        }
        if (Input.GetKey("2"))
        {
            rotation_flag = false;
            Debug.Log("switch");
        }
    }
}
