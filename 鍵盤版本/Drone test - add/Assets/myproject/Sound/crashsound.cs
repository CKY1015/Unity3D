using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class crashsound : MonoBehaviour
{

    AudioSource hitSound;
    void Start()
    {
        hitSound = GetComponent<AudioSource>();
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Picker"))
            hitSound.Play();
    }

}
