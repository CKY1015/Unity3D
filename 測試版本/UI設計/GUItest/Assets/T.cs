using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Euler(0, 45, 0);
	}
}
