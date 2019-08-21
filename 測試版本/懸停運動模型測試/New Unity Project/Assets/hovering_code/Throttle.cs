using System;
using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class Throttle : MonoBehaviour {


    private SerialPort sp;
    private Thread receiveThread;
    private static Rigidbody rb;

    string[] PWM_string = new string[5];
    double[] PWM_value = new double[5];
    double Trottle_force = 0;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }
    
    void Start()
    {
        if (sp != null && sp.IsOpen)
        {
            Debug.Log("close");
            sp.Close();
            sp.Dispose();
        }
        sp = new SerialPort("COM3", 9600)
        {
            ReadTimeout = 500
        };
        sp.Open();

        receiveThread = new Thread(ReceiveThread)
        {
            IsBackground = true
        };
        receiveThread.Start();

    }

    void Update()
    {
        Throttle_force();
    }

    private void ReceiveThread()
    {
        while (true)
        { 
            if (sp != null && sp.IsOpen)
            {
                try
                {
                    String strRec = sp.ReadLine();           
                    //Debug.Log("Receive From Serial: " + strRec);
                    if (strRec != "")
                    {
                        PWM_string = strRec.Split(',');
                        //Debug.Log(PWM_string.Length);
                        if(PWM_string.Length == 5)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                PWM_value[i] = Convert.ToDouble(PWM_string[i]);
                            }
                            //Debug.Log("PWM = " + PWM_value[0]);
                            Trottle_force = PWM_value[0];
                        }
                    }
                    Thread.Sleep(1);
                }
                catch(Exception e)
                {
                    Debug.Log("Error: "+e);
                    Thread.Sleep(1);
                }
            }
        }
        
    }

    
    private void Throttle_force()
    {
        float m = 0.4f;
        float g = 9.8f;
        float magn = 5f;
        Vector3 direction = rb.transform.up;
        //1084~2036
        //油門推超過1300才視為開始起飛
        //分成四種情形
        //#1:升力超過重力
        //#2:升力等於重力
        //#3:升力小於重力
        //#4:實體旋翼機未起飛
        Debug.Log(Trottle_force);
        if (Trottle_force >= 1300 && Trottle_force <= 2050)
        {
            if(Trottle_force >= 1450 && Trottle_force <= 2040)//#1
            {
                Trottle_force = m * g * Trottle_force / 1400;
                rb.AddRelativeForce(direction * ((float)Trottle_force - m * g) * magn);
            }
            else if (Trottle_force >= 1350 && Trottle_force <= 1450)//#2
            {
                Trottle_force = m * g;
                rb.AddRelativeForce(direction * ((float)Trottle_force - m * g) * magn);
            }       
            else if (Trottle_force >= 1300 && Trottle_force <= 1350)//#3
            {
                Trottle_force = m * g * Trottle_force / 1400;
                rb.AddRelativeForce(direction * ((float)Trottle_force - m * g) * magn);
            }
            
            //Debug.Log("test:"+((float)Trottle_force - m * g));
        }
        else//#4
            rb.AddRelativeForce(direction * (-m*g));


    }

    void OnDisable()
    {
        Debug.Log("Trying to abort");
        //Unity在離開當前場景後會自動呼叫這個函數
        receiveThread.Abort();//強制中斷當前執行緒
    }

}
