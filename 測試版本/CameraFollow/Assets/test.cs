using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    public Transform target;
    private float distanceUp = 2f;
    private float distanceAway = 3f;
    public float smooth = 1f;//位置平滑移动值 

    private float distance = 20;
    private float disspeed = 5000;
    private float min_distance = 15;
    private float max_distance = 500;

    void Update()
    {

    }

    void LateUpdate()
    {

        Vector3 disRot = target.localEulerAngles;
        transform.localEulerAngles = Vector3.Slerp(transform.localEulerAngles, disRot, Time.deltaTime * smooth);
        //transform.LookAt(target);
    }
}
