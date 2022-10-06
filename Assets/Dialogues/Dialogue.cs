using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;


[CreateAssetMenu (fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue", order = 1)]
public class Dialogue : ScriptableObject
{
    public float dialogueSpeed = 0.3f;

    public string speakerName;
    public Color speakerNameColor = Color.white;

    public Sprite speakerIcon;
    public Sprite secondaryIcon = null;


    [TextArea(3,5)]
    public string[] dialogue;

    TMP_Text nameText;
    TMP_Text textBox;

    Image avatarBox;
    Image secondaryBox;

    public void DisplayBox()
    {
        GameObject g = Instantiate(Resources.Load("DialogueBox") as GameObject, GameObject.Find("Canvas").transform);
        textBox = g.transform.Find("TextBox").GetComponent<TMP_Text>();

        nameText = g.transform.Find("Name").GetComponent<TMP_Text>();
        nameText.text = speakerName;
        nameText.color = speakerNameColor;

        avatarBox = g.transform.Find("Avatar").GetComponent<Image>();
        avatarBox.sprite = speakerIcon;

        if (secondaryIcon != null)
        {
            secondaryBox = g.transform.Find("Second").GetComponent<Image>();            
            secondaryBox.sprite = secondaryIcon;
            secondaryBox.color = Color.white;
        }
    }
    void CloseBox()
    {
        Destroy(textBox.gameObject.transform.parent.gameObject); // Works I guess.
        textBox = null;
        nameText = null;

        avatarBox = null;
        secondaryBox = null;

        FindObjectOfType<AudioManager>().Play("TextBlip");
    }



    public IEnumerator scrollTextSingle(string s = null)
    {        
        DisplayBox();
        if (s == null)
        {
            s = dialogue[0];
        }
        textBox.text = "";
        for (int i = 0; i < s.Length; i++)
        {
            // Check for font brackets (<>)
            if (s[i] == '<')
            {
                int skip = 0;
                char check = s[i];
                while (check != '>')
                {
                    skip++;
                    check = s[i + skip];
                }
                for (int j = i; j < skip+1; j++)
                {
                    textBox.text += s[j];
                }
                i += skip;                
            }
            else
            {
                textBox.text += s[i];
            }            
            yield return new WaitForSeconds(0.1f * dialogueSpeed);
        }
        while (Input.touchCount < 0)
        {
            yield return null;
        }
        CloseBox();
        yield return null;
    }

    public IEnumerator scrollTextArray(string[] s = null)
    {
        DisplayBox();
        if (s == null)
        {
            s = dialogue;
        }        
        for (int i = 0; i < s.Length; i++)
        {
            textBox.text = "";
            for (int j = 0; j < s[i].Length; j++)
            {

                // Check for font brackets (<>) different than the other one.
                if (s[i][j] == '<')
                {
                    int skip = 0;
                    char check = s[i][j];
                    string full = "";
                    while (check != '>')
                    {
                        full += check;
                        skip++;
                        check = s[i][j + skip];
                    }
                    full += check;
                    textBox.text += full;
                    j += skip;
                }
                else
                {
                    textBox.text += s[i][j];
                }
                
                yield return new WaitForSeconds(0.1f * dialogueSpeed);
            }
            while (Input.touchCount == 0)
            {
                yield return null;
            }
        }
        CloseBox();
        yield return null;
    }
}