using UnityEngine;
using System.Collections;

public class camerax : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
    public Transform target;
    public float relativeHeigth = 0;
    public float xDistance = 5.0f;
    public float dampSpeed = 2;
    // Update is called once per frame
    void Update () {
        Vector3 newPos = target.position+ new Vector3(xDistance, relativeHeigth, 0);
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * dampSpeed);
    }
}
