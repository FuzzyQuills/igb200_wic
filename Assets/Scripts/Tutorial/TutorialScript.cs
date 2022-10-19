using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialScript : MonoBehaviour
{
    public Dialogue[] dialogue;

    private void Start()
    {
        StartCoroutine(SpeakeyDoey());
    }

    IEnumerator SpeakeyDoey()
    {
        for (int i = 0; i < dialogue.Length; i++)
        {
            yield return StartCoroutine(dialogue[i].scrollTextArray());
            
        }        
        yield return null;
    }

}
