using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hover : MonoBehaviour {

    public Text Roll_value;
    public Text Pitch_value;
    public Text X_value;
    public Text Z_value;

    private Rigidbody rb;
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    int Speed = 50;
    float Angle_z;
    float Angle_x;

    Vector3 v;
    Vector3 z = new Vector3(0, 0, 1);
    Vector3 x = new Vector3(1, 0, 0);
    Vector3 thetaphi;
    Vector3 xz;

    // Update is called once per frame
    void FixedUpdate () {
        Control_rotate();
        v = N_vector();
        Angle_z = Angle_intersection(v, z);
        Debug.Log("z_angle_intersection:" + Angle_z);
        Angle_x = Angle_intersection(v, x);
        Debug.Log("x_angle_intersection:" + Angle_x);

        thetaphi = Get_rotate_angle();

        Add_force(Angle_z, z, thetaphi);
        Add_force(Angle_x, x, thetaphi);
        
        xz = Get_position();

        //UI angle demo
        Roll_value.text = thetaphi.z.ToString("0.00");
        Pitch_value.text = thetaphi.x.ToString("0.00");
        //UI position demo
        X_value.text = xz.x.ToString("0.00");
        Z_value.text = xz.z.ToString("0.00");
    }

    void Control_rotate()
    {
        if (Input.GetKey("up"))
        {
            transform.Rotate(Vector3.right * Time.deltaTime * Speed);
        }
        else if (Input.GetKey("down"))
        {
            transform.Rotate(Vector3.left * Time.deltaTime * Speed);
        }
        if (Input.GetKey("right"))
        {
            transform.Rotate(Vector3.back * Time.deltaTime * Speed);
        }
        else if (Input.GetKey("left"))
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * Speed);
        }
    }

    Vector3 N_vector()//法向量
    {
        Vector3 moveDirection = transform.up;
        //print("n_vector:" + "(" + moveDirection.x + "," + moveDirection.y + "," + moveDirection.z + ")");
        //Debug.Log(moveDirection);
        return moveDirection;
    }

    Vector3 Get_rotate_angle()//取得目前角度
    {
        Vector3 Eulerangle;
        Eulerangle = transform.localEulerAngles;
        if (Eulerangle.x > 180)
        {
            Eulerangle.x = Eulerangle.x - 360;
        }
        if (Eulerangle.y > 180)
        {
            Eulerangle.y = Eulerangle.y - 360;
        }
        if (Eulerangle.z > 180)
        {
            Eulerangle.z = Eulerangle.z - 360;
        }
        //print("Eulerangle:" + "(" + Eulerangle.x + "," + Eulerangle.y + "," + Eulerangle.z + ")");
        return Eulerangle;
    }

    Vector3 Get_position()//取得目前位置
    {
        Vector3 position;
        position = transform.localPosition;
        return position;
    }

    float Angle_intersection(Vector3 A,Vector3 B)
    {
        float angle;
        //theta = acos(v1.v2/|v1||v2|)
        float Inner_product;
        //v1.v2
        Inner_product = A.x * B.x + A.y * B.y + A.z * B.z;
        //|v1|
        float v1;
        v1 = Mathf.Sqrt(Mathf.Pow(A.x,2) + Mathf.Pow(A.y, 2) + Mathf.Pow(A.z, 2));
        //|v2|
        float v2;
        v2 = Mathf.Sqrt(Mathf.Pow(B.x, 2) + Mathf.Pow(B.y, 2) + Mathf.Pow(B.z, 2));
        //acos
        angle = Mathf.Acos(Inner_product / v1*v2);
        angle = (180 / Mathf.PI) * angle;
        return angle;//degree
    }

    
    void Add_force(float angle, Vector3 direction, Vector3 thetaphi)
    {
        float m = 0.4f;
        float g = 9.8f;
        float Zforce = m * g / Mathf.Cos(thetaphi.x * Mathf.PI / 180) / Mathf.Cos(thetaphi.z * Mathf.PI / 180);
        //Debug.Log(Zforce);
        rb.AddForce(direction*(Mathf.Abs(Zforce) * Mathf.Cos(angle * Mathf.PI / 180)));
    }

}
