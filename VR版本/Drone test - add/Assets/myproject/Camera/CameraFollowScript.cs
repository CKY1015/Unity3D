using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour {

    public Transform target;
    private float distanceUp = 7f;
    private float distanceAway = 20f;
    public float smooth = 20f;//位置平滑移动值 

    private float distance = 20;
    private float disspeed = 5000;
    private float min_distance = 15;
    private float max_distance = 500;

    void LateUpdate()
    {
        distance -= Input.GetAxis("Mouse ScrollWheel") * disspeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, min_distance, max_distance);
        //相機的位置 
        Vector3 disPos = target.position + Vector3.up * distance - target.forward * distanceAway ;
        transform.position = Vector3.Lerp(transform.position, disPos, Time.deltaTime*smooth);
        //相機的角度 
        transform.LookAt(target.position);
    }
}
