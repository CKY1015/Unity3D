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
        else
        {
            sp = new SerialPort("COM5", 9600)
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

    }

    void FixedUpdate()
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
        float m = 0.45f;
        float g = 9.8f;
        float magn = 1f;
        Vector3 direction = rb.transform.up;
        //1084~2036
        //油門推超過1300才視為開始起飛
        //分成四種情形
        //#1:升力超過重力
        //#2:升力等於重力
        //#3:升力小於重力
        //#4:實體旋翼機未起飛
        Trottle_force = 1.81f * Math.Pow(10, -5) * Math.Pow(PWM_value[0], 2) - 4.39f * Math.Pow(10, -2) * PWM_value[0] + 26.6f;
        Trottle_force = 4 * Trottle_force;
        //Debug.Log("total force = " + Trottle_force + "PWM_value[0] = " + PWM_value[0]);
        if (PWM_value[0] >= 1300 && PWM_value[0] <= 2050)
        {
            if (Trottle_force > m * g + 3)//#1
            {
                //Debug.Log("1");
                rb.AddRelativeForce(direction * ((float)Trottle_force - m * g) * magn);
            }
            else if (Trottle_force > m * g - 3 && Trottle_force < m * g + 3)//#2
            {
                //Debug.Log("2");
                Trottle_force = m * g;
                rb.AddRelativeForce(direction * ((float)Trottle_force - m * g) * magn);
            }
            else if (Trottle_force < m * g - 3)//#3
            {
                //Debug.Log("3");
                rb.AddRelativeForce(direction * ((float)Trottle_force - m * g) * magn);
            }

            //Debug.Log("test:"+((float)Trottle_force - m * g));
        }
        else//#4
        {
            if(PWM_value[0] == 0)//IOException
            {
                rb.AddRelativeForce(direction * 0);
            }
            else
            {
                rb.AddRelativeForce(direction * (-m * g));
            }    
            
        }   


    }

    void OnDisable()
    {
        Debug.Log("Trying to abort");
        //Unity在離開當前場景後會自動呼叫這個函數
        receiveThread.Abort();//強制中斷當前執行緒
    }

}
