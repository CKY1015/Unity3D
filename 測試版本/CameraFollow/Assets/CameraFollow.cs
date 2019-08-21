using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform target;
    public float x;
    public float y;
    private float xSpeed = 100;
    private float ySpeed = 100;
    private float distance = 5;
    private float disspeed = 200;
    private float min_distance = 3;
    private float max_distance = 10;

    private Quaternion rotationEuler;
    private Vector3 cameraPosotion;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
        y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;

        if(x > 360)
        {
            x -= 360;
        }
        else
        {
            x += 360;
        }

        distance -= Input.GetAxis("Mouse ScrollWheel") * disspeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, min_distance, max_distance);
        Debug.Log(distance);

        rotationEuler = Quaternion.Euler(y, x, 0);
        cameraPosotion = rotationEuler * new Vector3(0, 0, -distance) + target.position;

        transform.rotation = rotationEuler;
        transform.position = cameraPosotion;
    }

}
