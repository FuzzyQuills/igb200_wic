using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeScript : MonoBehaviour
{

    public PipeData thisPD;


    public bool rotateEnabled = true;

    Color connected = Color.green, 
          disconnected = Color.Lerp(Color.yellow, Color.red, 0.5f), 
          full = new Color32(0, 220, 220, 255); // Tea;

    public Renderer stateRenderer;

    public int state = 0; // 0 = disconnected, 1 = connected, 2 = full (had water in it and cannot move)

    public bool[] directions = new bool[4];

    public bool interactive = true;



    private void Start()
    {
        UpdateState(state);
        int r = Random.Range(0,3);
        for (int i = 0; i < r; i++)
        {
            ChangeDirection(false);
        }

    }

    public void UpdateState(int State)
    {
        state = State;
        switch (state)
        {
            case 0:
                stateRenderer.material.color = disconnected;
                break;
            case 1:
                stateRenderer.material.color = connected;
                break;
            case 2:
                stateRenderer.material.color = full;
                break;
            default:
                break;
        }
    }


    // Rotate when touched
    private void OnMouseDown()
    {
        if (interactive)
        {
            ChangeDirection(true);
        }        
    }

    void ChangeDirection(bool Bool)
    {
        if (rotateEnabled)
        {
            transform.Rotate(Vector3.forward, 90);
            directions = Shift(directions);
            if (Bool)
            {
                GameObject.FindObjectOfType<PlumbingGame>().UpdateStuff();
            }            
        }        
    }



    // Function that rotates array elements
    bool[] Shift(bool[] myArray)
    {
        bool[] tArray = new bool[myArray.Length];
        for (int i = 0; i < myArray.Length; i++)
        {
            if (i < myArray.Length - 1)
                tArray[i] = myArray[i + 1];
            else
                tArray[i] = myArray[0];
        }
        return tArray;
    }


}
