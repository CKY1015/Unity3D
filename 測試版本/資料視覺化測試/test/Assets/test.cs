using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {
    public int MoveSpeed = 10;
    public Transform target;
    public Rigidbody rb;
    public Vector3 targetPosition;
    public Vector3[] path;
    public float moveSpeed;
    public float x,y;
    public float speed = 100.0f;
    public Rigidbody s;
    // Use this for initialization
    void Start()
    {
        //gameObject.GetComponent<Rigidbody>().velocity = Vector3.forward * MoveSpeed;
        rb = GetComponent<Rigidbody>();
        //StartCoroutine(CountSeconds());
        StartCoroutine(MoveToPosition(targetPosition));
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log("transform.position:"+transform.position);
        //让物体向前运动Time.deltaTime距离
        //rb.MovePosition(transform.position + transform.forward * Time.deltaTime);
        if (Input.GetMouseButton(0))
        {//鼠标按着左键移动

            y = Input.GetAxis("Mouse X") * Time.deltaTime * speed;

            x = Input.GetAxis("Mouse Y") * Time.deltaTime * speed;

        }
        else
        {

            x = y = 0;

        }
        transform.Rotate(new Vector3(x, y, 0));
    }

    IEnumerator CountSeconds()
    {
        int seconds = 0;

        while (true)
        {
            for (float timer = 0; timer < 1; timer += Time.deltaTime)
                yield return 0;

            seconds++;
            Debug.Log(seconds + " seconds have passed since the Coroutine started.");
        }
    }

    IEnumerator MoveToPosition(Vector3 target)
    {
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return 0;
        }
    }
}
