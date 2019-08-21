using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Redfollow : MonoBehaviour {

    public Rigidbody drone;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var p = transform.localPosition;
        p.x = drone.position.x;
        p.z = drone.position.z;
	}
}
