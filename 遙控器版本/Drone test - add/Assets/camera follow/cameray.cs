using UnityEngine;
using System.Collections;

public class cameray : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    public Transform target;
    public float relativeHeigth = 10.0f;
    public float yDistance = 5.0f;
    public float dampSpeed = 2;
    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = target.position + new Vector3(0, yDistance, 0);
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * dampSpeed);
    }
}
