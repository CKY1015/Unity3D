using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine.UI;

public class rotation : MonoBehaviour {

    public Text pitch_text;
    public Text roll_text;
    public Text yaw_text;
    public double pitch;
    public double roll;
    public double yaw;
    public int p1;
    public int r1;
    public int y1;
    public int y2;
    

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        /*
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Rotate(Vector3.right * Time.deltaTime * 30);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Rotate(Vector3.left * Time.deltaTime * 30);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * 30);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.back * Time.deltaTime * 30);
        }
        if (Input.GetKey(KeyCode.RightControl))
        {
            transform.Rotate(Vector3.down * Time.deltaTime * 30);
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 30);
        }
        else
        {
            yaw = transform.localRotation.y;
            yaw = yaw / (0.7071067) * 90;
            y1 = (int)yaw;
        }*/
        yaw = transform.localRotation.y;
        yaw = yaw / (0.7071067) * 90;
        y2 = (int)yaw;
       

      

        pitch = transform.localRotation.z;
        roll = transform.localRotation.x;
        
        pitch = pitch / (0.7071067) * 90;
        roll = roll / (0.7071067) * 90;
        p1 = (int)pitch;    //change to int
        r1 = (int)roll;
        //print("X角度为：" + this.transform.localEulerAngles.x);
        //print("Y角度为：" + this.transform.localEulerAngles.y);
        //print("Z角度为：" + this.transform.localEulerAngles.z);

        /*
                Slider_1.value = 900 +(p1) *10+(-r1) * 10 + (-y2 + y1) * 10;   //右前  900是基礎PWM訊號
                Slider_2.value = 900 +(p1)  *10+(r1) * 10 + (y2 - y1) * 10;   //右後
                Slider_3.value = 900 +(-p1) *10+ (r1) * 10 + (-y2 + y1) * 10;   //左前
                Slider_4.value = 900 +(-p1)  *10+ (-r1) * 10 + (y2 - y1) * 10;    //左後
                bar1 = Slider_1.value;
                bar1_power.text = "" + bar1;
                bar2 = Slider_2.value;
                bar2_power.text = "" + bar2;
                bar3 = Slider_3.value;
                bar3_power.text = "" + bar3;
                bar4 = Slider_4.value;
                bar4_power.text = "" + bar4;

        */


        pitch_text.text = "" + p1;
        roll_text.text = "" + r1;
        yaw_text.text = "" + y2;


        

        

    }
}



////
/*
 * 
 * 
     public Text pitch_text;
    public Text roll_text;
    public Text yaw_text;
    public double pitch;
    public double roll;
    public double yaw;
    public int p1;
    public int r1;
    public int y1;
    public double bar1;
    public double bar2;
    public double bar3;
    public double bar4;
    public Slider Slider_1;
    public Slider Slider_2;
    public Slider Slider_3;
    public Slider Slider_4;
    public Text bar1_power;
    public Text bar2_power;
    public Text bar3_power;
    public Text bar4_power;
 * 
 * 
 * 
 * 
 * 
pitch = transform.localRotation.z;
        roll = transform.localRotation.x;
        yaw = transform.localRotation.y;
        pitch = pitch / (0.7071067) * 90;
        roll = roll / (0.7071067) * 90;
        yaw = yaw / (0.7071067) * 90;
        p1 = (int)pitch;    //change to int
        r1 = (int)roll;
        y1 = (int)yaw;


        Slider_1.value = -890+900 + (-p1) * 10 + (-r1) * 10 + (y1) * 10;   //右前  900是基礎PWM訊號
        Slider_2.value = 890+900 + (p1) * 10 + (-r1) * 10 + (-y1) * 10;   //右後
        Slider_3.value = 890+900 + (-p1) * 10 + (r1) * 10 + (-y1) * 10;   //左前
        Slider_4.value = -890+900 + (p1) * 10 + (r1) * 10 + (y1) * 10;    //左後
        bar1 = Slider_1.value;
        bar1_power.text = "" + bar1;
        bar2 = Slider_2.value;
        bar2_power.text = "" + bar2;
        bar3 = Slider_3.value;
        bar3_power.text = "" + bar3;
        bar4 = Slider_4.value;
        bar4_power.text = "" + bar4;
          pitch_text.text = "" + p1;
        roll_text.text = "" + r1;
        yaw_text.text = "" + (y1-89);
*/
