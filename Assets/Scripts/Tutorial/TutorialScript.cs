using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialScript : MonoBehaviour
{
    //public Dialogue[] dialogue;
    public DialogueScene[] dialogueScene;

    private void Start()
    {
        StartCoroutine(SpeakeyDoey());
    }

    IEnumerator SpeakeyDoey(int a = 0)
    {
        for (int i = a; i < dialogueScene.Length; i++)
        {
            GameObject.Find("bgSprite").GetComponent<Image>().sprite = dialogueScene[i].background;

            for (int j = 0; j < dialogueScene[i].dialogues.Length; j++)
            {
                yield return StartCoroutine(dialogueScene[i].dialogues[j].scrollTextArray());
            }
        }        
        //for (int i = 0; i < dialogue.Length; i++)
        //{
        //    yield return StartCoroutine(dialogue[i].scrollTextArray());
            
        //}        
        yield return null;
    }

    [System.Serializable]
    public class DialogueScene
    {
        public Dialogue[] dialogues;
        public Sprite background;

        public DialogueScene() { }
    }

    public void SkipForward(int a)
    {
        StopAllCoroutines();
        if (GameObject.Find("DialogueBox(Clone)"))
        {
            Destroy(GameObject.Find("DialogueBox(Clone)"));
        }
        StartCoroutine(SpeakeyDoey(a));
    }
}
