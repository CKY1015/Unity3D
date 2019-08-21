using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class yaw_class : MonoBehaviour
{
    public Text yawText;
    public double yaw;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        yawText.text = "yaw_angle";
        yaw = transform.localRotation.y;
        yaw = yaw / (0.7071067) * 90;
        yawText.text = "" + yaw;


    }
}
