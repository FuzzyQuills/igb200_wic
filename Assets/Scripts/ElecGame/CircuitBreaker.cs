using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CircuitBreaker : MonoBehaviour
{
    public Animator anim;
    public bool IsOn = false;
    public bool disableBreaker = false;

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("OnOrOff", IsOn);
        if (IsOn)
        {
            transform.GetChild(1).GetComponent<Renderer>().material.color = Color.yellow;
            FindObjectOfType<AudioManager>().Play("ButtonClick");
        }
        else
        {
            transform.GetChild(1).GetComponent<Renderer>().material.color = Color.white;
            FindObjectOfType<AudioManager>().Play("ButtonClick");
        }
    }

    public void OnMouseDown() {
        if (!disableBreaker) IsOn = !IsOn;
    }
}
