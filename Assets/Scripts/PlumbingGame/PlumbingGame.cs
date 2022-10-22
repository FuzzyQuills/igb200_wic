using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlumbingGame : MonoBehaviour
{
    public int gridX = 4, gridY = 4; // Must be even.
    public float scale = 1f; // changes size of tiles
    public List<PipeData> pipes = new List<PipeData>();


    public GameObject[] generates; // A list of pipe gameobjects to be spawned from.

    GameObject codeMan;
    TileInfoCollector tIC;
    public GameData gd;

    int stars = 5;
    public float maxTime = 15;
    float currentTime = 15;
    bool timerStopper = false;
    public Slider timerSlider;
    public TMP_Text starText;
    public TMP_Text winText;

    private void Start()
    {
        // Game Data stuff
        codeMan = GameObject.Find("CodeMan");
        tIC = codeMan.GetComponent<TileInfoCollector>();
        gd = codeMan.GetComponent<GameData>();


        if (gd)
        {
            gd.expenditure = 0;            
        }
        if (tIC)
        {            
            maxTime = maxTime - tIC.currentLevel;            
        }       

        //// Script for scaling the game's pipes and size

        //if (codeMan != null)
        //{
        //    gridX = gridX + codeMan.currentLevel * 2;
        //    gridY = gridY + codeMan.currentLevel * 2;
        //    scale = scale + (codeMan.currentLevel * 0.45f); // Should be 0.5f for an even, infinite division
        //}

        // Spawn the pipes in random positions
        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                int r = Random.Range(0,generates.Length);
                GameObject g = Instantiate(generates[r], transform);
                g.transform.position = new Vector3((i+0.5f - (gridX / 2)) + transform.position.x, -(j+0.5f - (gridY / 2)) + transform.position.y, 0) / scale;
                g.transform.localScale = Vector3.one * 0.95f / scale;

                
                pipes.Add(new PipeData(g, new Vector2(i,j)));
            }
        }

        
        for (int i = 0; i < pipes.Count; i++)
        {
            pipes[i].pipeScript.thisPD = pipes[i];
        }


        GameObject start = Instantiate(Resources.Load("Pipes/PipeStart") as GameObject, transform);
        start.transform.position = new Vector3((0.5f - (gridX/2)) + transform.position.x, (gridY+0.5f - (gridY/2)) + transform.position.y, 0) / scale;
        start.transform.localScale = Vector3.one * 0.95f / scale;
        GameObject end =  Instantiate(Resources.Load("Pipes/PipeEnd") as GameObject, transform);
        end.transform.position = new Vector3((gridX-0.5f - (gridX/2)) + transform.position.x, (-0.5f - (gridY/2)) + transform.position.y, 0) / scale;
        end.transform.localScale = Vector3.one * 0.95f / scale;

        for (int i = 0; i < pipes.Count; i++)
        {
            GiveNeihbours(pipes[i]);
        }


        // Delete some boxes around pipes. Amount reduces with game level.
        for (int i = 0; i < 10 - tIC.currentLevel; i++)
        {
            while (true)
            {
                int a = Random.Range(0, pipes.Count);
                if (pipes[a].pipeScript.mysteryCube != null)
                {
                    Destroy(pipes[a].pipeScript.mysteryCube);
                    break;
                }
            }
        }

        currentTime = maxTime;
        timerSlider.maxValue = maxTime;
        timerSlider.value = currentTime;

        Invoke("UpdateStuff", 0.1f);
    }

    private void Update()
    {
        if (stars > 1 && !timerStopper)
        {
            currentTime -= Time.deltaTime;
            timerSlider.value = currentTime;
            if (currentTime <= 0)
            {
                stars--;
                starText.text = $"{stars} stars";
                currentTime = maxTime;
            }
        }        
    }


    /// <summary>
    /// Reverts all pipes to state zero and then 'reinfects' valid pipes. Also called in PipeScript whenever a pipe is turned.
    /// </summary>
    public void UpdateStuff()
    {
        foreach (PipeData p in pipes)
        {
            p.pipe.GetComponent<PipeScript>().UpdateState(0);
        }


        foreach (PipeData p in pipes)
        {
            if (p.pos == Vector2.zero)
            {                
                if (p.pipe.GetComponent<PipeScript>().directions[0] == true)
                {
                    p.pipe.GetComponent<PipeScript>().UpdateState(1);
                    // Do recursive?
                    Infect(p);
                }             
            }

            // Win Condition
            if (p.pos == new Vector2(gridX - 1, gridY - 1))
            {                
                if (p.pipeScript.mysteryCube == null && p.pipeScript.state == 1 && p.pipeScript.directions[2] == true)
                {
                    int reward = GameData.Reward(stars);
                    // Display win message
                    timerStopper = true; // Stop timer
                    Debug.Log("Win!");
                    GameObject.Find("Squibble").GetComponent<TMP_Text>().text = "You Win!";
                    winText.text = $"{stars} Stars!<br><color=green>{reward}k awarded!";
                    // Reward finances
                    gd = GameObject.FindObjectOfType<GameData>();
                    if (gd)
                    {
                        gd.expenditure += reward;
                        gd.starsOnLevel[gd.playlistOrder - 1] = stars;
                    }
                    // Disable interactions with pipes, make connecting pipes blue
                    foreach (PipeData pp in pipes)
                    {
                        pp.pipeScript.interactive = false;
                        if (pp.pipeScript.state == 1)
                        {
                            pp.pipeScript.UpdateState(2);                            
                        }
                    }
                    break;
                }
                
            }
        }
    }

    /// <summary>
    /// This is where the game makes the pipes go green if they connect.
    /// Shit name tho.
    /// </summary>
    public void Infect(PipeData pD)
    {
        if (pD.pipeScript.mysteryCube != null)
        {
            return;
        }
        //North
        if (pD.neighbours[0]?.state == 0 && pD.neighbours[0].directions[2] == true)
        {
            if (pD.pipeScript.directions[0] == true)
            {
                pD.neighbours[0].UpdateState(1);
                Infect(pD.neighbours[0].thisPD);
            }
        }
        //East
        if (pD.neighbours[1]?.state == 0 && pD.neighbours[1].directions[3] == true)
        {
            if (pD.pipeScript.directions[1] == true)
            {
                pD.neighbours[1].UpdateState(1);
                Infect(pD.neighbours[1].thisPD);
            }
        }
        // South
        if (pD.neighbours[2]?.state == 0 && pD.neighbours[2].directions[0] == true)
        {
            if (pD.pipeScript.directions[2] == true)
            {
                pD.neighbours[2].UpdateState(1);
                Infect(pD.neighbours[2].thisPD);
            }
        }
        if (pD.neighbours[3]?.state == 0 && pD.neighbours[3].directions[1] == true)
        {
            if (pD.pipeScript.directions[3] == true)
            {
                pD.neighbours[3].UpdateState(1);
                Infect(pD.neighbours[3].thisPD);
            }
        }
    }    

    /// <summary>
    /// For getting the surrounding pipes of a given pipe
    /// </summary>
    /// <param name="p"></param>
    void GiveNeihbours(PipeData p)
    {
        foreach (PipeData pipe in pipes)
        {
            // North neighbour
            if (pipe.pos == p.pos - Vector2.up)
            {
                p.neighbours[0] = pipe.pipeScript;
            }
            // South neighbour
            if (pipe.pos == p.pos - Vector2.down)
            {
                p.neighbours[2] = pipe.pipeScript;
            }
            // East neighbour
            if (pipe.pos == p.pos + Vector2.right)
            {
                p.neighbours[1] = pipe.pipeScript;
            }
            // West neighbour
            if (pipe.pos == p.pos + Vector2.left)
            {
                p.neighbours[3] = pipe.pipeScript;
            }
        }
    }
}

[System.Serializable]
public class PipeData
{
    public PipeData(GameObject G, Vector2 Vec)
    {
        pipe = G;
        pos = Vec;
        pipeScript = G.GetComponent<PipeScript>();
    }
    public GameObject pipe;
    public Vector2 pos;
    public PipeScript pipeScript;
    public PipeScript[] neighbours = new PipeScript[4];

}
