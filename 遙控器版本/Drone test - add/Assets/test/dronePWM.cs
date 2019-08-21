using System;
using UnityEngine;
using System.Collections;
using System.IO.Ports;


public class dronePWM : MonoBehaviour
{


    Rigidbody ourDrone;
    public SerialPort sp = new SerialPort("COM3", 9600);
    string[] PWM_string = new string[4];
    double[] PWM_value = new double[4];

    public double yaw;


    void Awake()
    {
        ourDrone = GetComponent<Rigidbody>();
    }

    public void Start()
    {
        yaw = transform.localRotation.y;

        sp.Open();
        sp.ReadTimeout = 1;
    }

    void FixedUpdate()
    {
        //Debug.Log("TIME:" + Time.time);
        if (sp.IsOpen)
        {
            try
            {
                string value = sp.ReadLine();
                if (value != "")
                {
                   // Debug.Log("PWM=" + value);
                    PWM_string = value.Split(',');
                    for (int i = 0; i < 4; i++)
                    {
                        PWM_value[i] = Convert.ToDouble(PWM_string[i]);
                        //Debug.Log("PWM=" + PWM_value[0]);
                    }
                }
            }
            catch (Exception e)
            {
                //Debug.Log("" + e);
            }
        }

        if (Time.time > 3)
        {
            MovementUpDown();
            MovementForward();
            Rotation();
            ClampingSpeedValues();
            Swerve();
        }

        ourDrone.AddRelativeForce(Vector3.up * upForce);
        ourDrone.rotation = Quaternion.Euler(
            new Vector3(tiltAmountFoward, currentYRotation, tiltAmountSideways)
            );


    }

    public float upForce;
    void MovementUpDown()
    {
        Debug.Log("Input.GetAxis(H)):" + Input.GetAxis("Horizontal"));
        if ((Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f))
        {
            if ((float)PWM_value[0] > 1700 || (float)PWM_value[0] < 1200)
            {
                ourDrone.velocity = ourDrone.velocity;
            }
            if ((float)PWM_value[0] < 1700 && (float)PWM_value[0] > 1200 && (float)PWM_value[1] > 1200 && (float)PWM_value[1] < 1700)
            {
                ourDrone.velocity = new Vector3(ourDrone.velocity.x, Mathf.Lerp(ourDrone.velocity.y, 0, Time.deltaTime * 5), ourDrone.velocity.z);
                upForce = 281;
            }
            if ((float)PWM_value[0] < 1700 && (float)PWM_value[0] > 1200 && (float)PWM_value[1] < 1200 || (float)PWM_value[1] > 1700)
            {
                ourDrone.velocity = new Vector3(ourDrone.velocity.x, Mathf.Lerp(ourDrone.velocity.y, 0, Time.deltaTime * 5), ourDrone.velocity.z);
                upForce = 110;
            }
            if ((float)PWM_value[1] < 1200 || (float)PWM_value[1] > 1700)
            {
                upForce = 410;
            }
        }
        if ((float)PWM_value[0] > 1700)
        {
            upForce = 450;
            if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f)
            {
                upForce = 500;
            }
        }
        else if ((float)PWM_value[0] < 1200)
        {
            upForce = -200;
        }
        else if ((float)PWM_value[0] < 1700 && (float)PWM_value[0] > 1200 && (Mathf.Abs(Input.GetAxis("Vertical")) < -0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) < -0.2f))
        {
            upForce = -98.1f;
        }
    }
    private float movementForwardSpeed = 500.0f;
    private float tiltAmountFoward = 0;
    private float tiltVelocityForward;
    void MovementForward()
    {
        if (Input.GetAxis("Vertical") != 0)
        {
            ourDrone.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * movementForwardSpeed);
            tiltAmountFoward = Mathf.SmoothDamp(tiltAmountFoward, 20 * Input.GetAxis("Vertical"), ref tiltVelocityForward, 0.1f);//緩衝
        }

    }

    private float wantedYRotation;
    [HideInInspector]
    public float currentYRotation;
    private float rotateAmoutByKeys = 2.5f;
    private float rotationYVelocity;
    void Rotation()
    {
        if ((float)PWM_value[1] < 1200)
        {
            wantedYRotation -= rotateAmoutByKeys;
        }
        if ((float)PWM_value[1] > 1700)
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
        if (Mathf.Abs(Input.GetAxis("Vertical")) <-0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f)
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
        Debug.Log("test" + Mathf.Abs(Input.GetAxis("Horizontal")));
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


