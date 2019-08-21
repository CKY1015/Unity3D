using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour {

    public Transform target;
    private float distanceAway = 20f;
    private float distanceUp = 10;
    public float smooth = 100f;//位置平滑移动值 
    private float disspeed = 5000;
    private float min_distance = 5;
    private float max_distance = 500;


    void FixedUpdate()
    {
        distanceUp -= Input.GetAxis("Mouse ScrollWheel") * disspeed * Time.deltaTime;
        distanceUp = Mathf.Clamp(distanceUp, min_distance, max_distance);
        //相機的位置 
        Vector3 disPos = target.position + Vector3.up * distanceUp - target.forward * distanceAway;
        //Debug.Log(target.position.y + distanceUp);
        disPos.y = target.position.y + distanceUp;
        transform.position = Vector3.Lerp(transform.position, disPos, Time.deltaTime * smooth);

        //相機的角度 
        transform.LookAt(target.position);
    }
}
