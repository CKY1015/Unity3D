using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class roll_class : MonoBehaviour
{
    public Text rollText;
    public double roll;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        rollText.text = "roll_angle";
        roll = transform.localRotation.z;
        roll = roll / (-0.7071067) * 90;
        rollText.text = "" + roll;


    }
}
