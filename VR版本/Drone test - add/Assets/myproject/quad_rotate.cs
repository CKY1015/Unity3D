using UnityEngine;
using System.Collections;

public class quad_rotate : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Rotate(Vector3.down * Time.deltaTime * 30000);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 30000);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.down * Time.deltaTime * 30000);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 30000);
        }
        if (Input.GetKey(KeyCode.RightControl))
        {
            transform.Rotate(Vector3.down * Time.deltaTime * 30000);
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 30000);
        }
        if (Input.GetKey(KeyCode.I))
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 30000);
        }
        if (Input.GetKey(KeyCode.K))
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 30000);
        }
        if (Input.GetKey(KeyCode.L))
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 30000);
        }
        if (Input.GetKey(KeyCode.J))
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 30000);
        }
    }
}
