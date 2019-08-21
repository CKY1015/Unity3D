using System;
using UnityEngine;
using System.Collections;
using System.IO.Ports;



public class dronePWM : MonoBehaviour
{


    Rigidbody ourDrone;
    public SerialPort sp = new SerialPort("COM1", 9600);
    string[] PWM_string = new string[4];
    double[] PWM_value = new double[4];
    void Awake()
    {
        ourDrone = GetComponent<Rigidbody>();
    }

    public void Start()
    {
        sp.Open();
        sp.ReadTimeout = 1;
    }

    void FixedUpdate()
    {
        if(sp.IsOpen)
        {
            try
            {
                string value = sp.ReadLine();
                if(value != "")
                {
                    //Debug.Log("value=" + value);
                    //Debug.Log("a");
                    PWM_string = value.Split(',');
                    //Debug.Log(PWM_string);
                    for (int i = 0; i < 4; i++)
                    {
                        PWM_value[i] = Convert.ToDouble(PWM_string[i]);
                        Debug.Log("PWM=" + PWM_value[0]);
                    }
                    //Debug.Log("b");
                    MovementUpDown();

                }
                //Debug.Log("c");
            }
            catch(Exception e)
            {
                Debug.Log(""+e);
            }
        }
        //Debug.Log("d");
        //Debug.Log("ff");
        ourDrone.AddRelativeForce(Vector3.up * upForce);
        //MovementForward();
        /*
        MovementForward();
        Rotation();
        ClampingSpeedValues();
        Swerve();


        ourDrone.rotation = Quaternion.Euler(
            new Vector3(tiltAmountFoward, currentYRotation, tiltAmountSideways)
            );
            */

    }

    public float upForce;
    void MovementUpDown()
    {
        upForce = ((float)PWM_value[0] - 1500);
        Debug.Log("" + upForce);
        /*
        if (Input.GetKey(KeyCode.I))
        {
            upForce = 450;
        }
        else if (Input.GetKey(KeyCode.K))
        {
            upForce = -200;
        }
        else if (!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K))
        {
            upForce = 98.1f;
        }
        */
    }
    private float movementForwardSpeed = 500.0f;
    private float tiltAmountFoward = 0;
    private float tiltVelocityForward;
    void MovementForward()
    {
        float a = ((float)PWM_value[2] - 1500);
        ourDrone.AddRelativeForce(Vector3.forward * a * movementForwardSpeed);
        tiltAmountFoward = Mathf.SmoothDamp(tiltAmountFoward, 20 * a, ref tiltVelocityForward, 0.1f);//緩衝
        /*

        if (Input.GetAxis("Vertical") != 0)
        {
            ourDrone.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * movementForwardSpeed);
            tiltAmountFoward = Mathf.SmoothDamp(tiltAmountFoward, 20 * Input.GetAxis("Vertical"), ref tiltVelocityForward, 0.1f);//緩衝
        }
        */

    }

    private float wantedYRotation;
    private float currentYRotation;
    private float rotateAmoutByKeys = 2.5f;
    private float rotationYVelocity;
    void Rotation()
    {
        if (Input.GetKey(KeyCode.J))
        {
            wantedYRotation -= rotateAmoutByKeys;
        }
        if (Input.GetKey(KeyCode.L))
        {
            wantedYRotation += rotateAmoutByKeys;
        }
        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, 0.25f);
    }

    private Vector3 velocityToSmoothDampToZero;
    void ClampingSpeedValues()
    {
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f)
        {
            ourDrone.velocity = Vector3.ClampMagnitude(ourDrone.velocity, Mathf.Lerp(ourDrone.velocity.magnitude, 10.0f, Time.deltaTime * 5f));
        }
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) < 0.2f)
        {
            ourDrone.velocity = Vector3.ClampMagnitude(ourDrone.velocity, Mathf.Lerp(ourDrone.velocity.magnitude, 10.0f, Time.deltaTime * 5f));
        }
        if (Mathf.Abs(Input.GetAxis("Vertical")) < 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f)
        {
            ourDrone.velocity = Vector3.ClampMagnitude(ourDrone.velocity, Mathf.Lerp(ourDrone.velocity.magnitude, 5.0f, Time.deltaTime * 5f));
        }
        if (Mathf.Abs(Input.GetAxis("Vertical")) < 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) < 0.2f)
        {
            ourDrone.velocity = Vector3.SmoothDamp(ourDrone.velocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.95f);
        }
    }

    private float sideMovementAmount = 300.0f;
    private float tiltAmountSideways;
    private float tiltAmountVelocity;
    void Swerve()
    {
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f)
        {
            ourDrone.AddRelativeForce(Vector3.right * Input.GetAxis("Horizontal") * sideMovementAmount);
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, -20 * Input.GetAxis("Horizontal"), ref tiltAmountVelocity, 0.1f);
        }
        else
        {
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 0, ref tiltAmountVelocity, 0.1f);
        }
    }
}


