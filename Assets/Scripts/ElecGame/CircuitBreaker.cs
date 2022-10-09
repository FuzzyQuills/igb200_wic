using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitBreaker : MonoBehaviour
{
    public Animator anim;
    public bool IsOn;

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("OnOrOff", IsOn);
    }

    public void OnMouseDown() {
        IsOn = !IsOn;
    }
}
