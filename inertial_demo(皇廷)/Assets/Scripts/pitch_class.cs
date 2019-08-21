using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class pitch_class : MonoBehaviour {
    public Text pitchText;
    public double pitch;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        pitchText.text = "pitch_angle";
        pitch = transform.localRotation.x;
        pitch = pitch / (0.7071067) * 90;
        pitchText.text = "" + pitch;


	}
}
