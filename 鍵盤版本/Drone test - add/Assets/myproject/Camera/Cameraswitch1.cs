using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameraswitch1 : MonoBehaviour {

    public GameObject cm1, cm2;

    private void Awake()
    {
        cm1.SetActive(true);
        cm2.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKey("2"))
        {
            cm1.SetActive(false);
            cm2.SetActive(true);
        }
        if (Input.GetKey("1"))
        {
            cm1.SetActive(true);
            cm2.SetActive(false);
        }

    }
}
