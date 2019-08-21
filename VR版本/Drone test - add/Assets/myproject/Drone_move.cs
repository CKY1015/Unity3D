using UnityEngine;
using System.Collections;


public class Drone_move : MonoBehaviour {


    Rigidbody ourDrone;
    

    void Awake()
    {
        ourDrone = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        MovementUpDown();
        Rotation();
        ClampingSpeedValues();
        FBLR();
       
        ourDrone.AddRelativeForce(Vector3.up * upForce);
        ourDrone.rotation = Quaternion.Euler(
            new Vector3(tiltAmountFoward, currentYRotation, tiltAmountSideways)
            );
        //Debug.Log(Mathf.Abs(Input.GetAxis("Vertical")) +","+ Mathf.Abs(Input.GetAxis("Horizontal")));
        //Debug.Log(tiltAmountFoward + ","+ currentYRotation + ","+ tiltAmountSideways);
            
        
    }

    public float upForce;
    void MovementUpDown()
    {
        if((Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f))
        {
            if (!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && !Input.GetKey(KeyCode.J) && !Input.GetKey(KeyCode.L))
            {
                ourDrone.velocity = new Vector3(ourDrone.velocity.x, Mathf.Lerp(ourDrone.velocity.y, 0, Time.deltaTime * 5), ourDrone.velocity.z);
                upForce = 281;
            }
        }
        if (Input.GetKey(KeyCode.I))
        {
            upForce = 450;
        }
        else if (Input.GetKey(KeyCode.K))
        {
            upForce = -200;
        }
        else if (!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && (Mathf.Abs(Input.GetAxis("Vertical")) < 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) < 0.2f))
        {
            upForce = -98.1f;
        }
    }
    

    private float wantedYRotation = -60;
    private float currentYRotation = -60;
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
        if (Mathf.Abs(Input.GetAxis("Vertical")) >0.2f && Mathf.Abs(Input.GetAxis("Horizontal"))>0.2f)
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
            ourDrone.velocity = Vector3.SmoothDamp(ourDrone.velocity,Vector3.zero ,ref velocityToSmoothDampToZero,0.95f);
        }
    }

    private float sideMovementAmount = 500.0f;
    private float tiltAmountSideways = 0;
    private float tiltAmountVelocity;

    private float movementForwardSpeed = 500.0f;
    private float tiltAmountFoward = 0;
    private float tiltVelocityForward;
    void FBLR()
    {
        
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f)
        {
            ourDrone.AddRelativeForce(Vector3.right * Input.GetAxis("Horizontal") * sideMovementAmount);
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, -20 * Input.GetAxis("Horizontal"), ref tiltAmountVelocity, 0.1f);
        }
        else
        {
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 0 , ref tiltAmountVelocity, 0.5f);
        }

        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f)
        {
            ourDrone.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * movementForwardSpeed);
            tiltAmountFoward = Mathf.SmoothDamp(tiltAmountFoward, 20 * Input.GetAxis("Vertical"), ref tiltVelocityForward, 0.1f);//緩衝
        }
        else
        {
            tiltAmountFoward = Mathf.SmoothDamp(tiltAmountFoward, 0, ref tiltVelocityForward, 0.5f);
        }
    }

}


