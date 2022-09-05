using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PipeType {
    PipeL = 0,
    PipeL_alt = 1,
    PipeQuad = 2,
    PipeStraight = 3,
    PipeTri = 4
}

[System.Serializable]
public struct Pipe {
    public PipeType pipe;
    public int orientation; // 0 = north, 1 = east, 2 = south, 3 = west
}

/* Comment for Coen:
 * Ideally, the process this script would execute would be:
 * - Wait for player input.
 * - When a player drags a pipe, wait for the touch release event.
 * - After touch release, place the pipe, recalculate the path.
 * - If (path start can get to path end) player wins this minigame.
 * Obviously fail the game if the time runs out.
 * I made this skeleton before my brain basically said no, hope this helps. 
 */
public class PipesMinigameManager : MonoBehaviour
{
    public GameObject PipeL;
    public GameObject PipeL_alt;
    public GameObject PipeQuad;
    public GameObject PipeStraight;
    public GameObject PipeTri;
    public Pipe[] pipes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
