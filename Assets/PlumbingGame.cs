using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlumbingGame : MonoBehaviour
{
    public int x = 4, y = 4; // Must be even.
    public float scale = 1f; // changes size of tiles
    public List<PipeData> pipes = new List<PipeData>();


    public GameObject[] generates; // A list of pipe gameobjects to be spawned from.

    TileInfoCollector codeMan;

    private void Start()
    {
        codeMan = GameObject.FindObjectOfType<TileInfoCollector>();
        if (codeMan != null)
        {
            x = x + codeMan.currentLevel * 2;
            y = y + codeMan.currentLevel * 2;
            scale = scale + (codeMan.currentLevel * 0.45f); // Should be 0.5f for an even, infinite division
        }


        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                int r = Random.Range(0,generates.Length);
                GameObject g = Instantiate(generates[r], transform);
                g.transform.position = new Vector3(i+0.5f - (x / 2), -(j+0.5f - (y / 2)), 0) / scale;
                g.transform.localScale = Vector3.one * 0.95f / scale;

                
                pipes.Add(new PipeData(g, new Vector2(i,j)));
            }
        }

        
        for (int i = 0; i < pipes.Count; i++)
        {
            pipes[i].pipeScript.thisPD = pipes[i];
        }


        GameObject start = Instantiate(Resources.Load("Pipes/PipeStart") as GameObject, transform);
        start.transform.position = new Vector3(0.5f - (x/2), y+0.5f - (y/2), 0) / scale;
        start.transform.localScale = Vector3.one * 0.95f / scale;
        GameObject end =  Instantiate(Resources.Load("Pipes/PipeEnd") as GameObject, transform);
        end.transform.position = new Vector3(x-0.5f - (x/2), -0.5f - (y/2), 0) / scale;
        end.transform.localScale = Vector3.one * 0.95f / scale;

        for (int i = 0; i < pipes.Count; i++)
        {
            GiveNeihbours(pipes[i]);
        }


        UpdateStuff();
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

            if (p.pos == new Vector2(x - 1, y - 1))
            {                
                if (p.pipeScript.state == 1 && p.pipeScript.directions[2] == true)
                {
                    Debug.Log("Win!");
                    foreach (PipeData pp in pipes)
                    {
                        if (pp.pipeScript.state == 1)
                        {
                            pp.pipeScript.UpdateState(2);
                        }
                    }
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
