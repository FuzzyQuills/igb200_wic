using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ElecNode : MonoBehaviour
{
    public TMP_Text voltageReading;
    public CircuitBreaker breaker;
    public float voltageDiff;

    public void Update () {
        if (breaker.IsOn) {
            voltageReading.text = string.Format("ON\n{0:0.00}V",voltageDiff);
        } else {
            voltageReading.text = string.Format("OFF\n{0:0.00}V",voltageDiff);
        }
    }
}
