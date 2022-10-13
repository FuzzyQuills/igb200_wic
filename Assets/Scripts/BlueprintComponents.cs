using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Script held in the blueprint scene. For general stuff
/// </summary>
public class BlueprintComponents : MonoBehaviour
{
    bool tilesOrNodes = false; // 0=tiles, 1=nodes
    public GameObject[] holders; // Holds the TileHolder and NodeHolder interfaces for enabling/disabling

    public TMP_Text throwaway; // for alpha Emily's text bubble.

    public TMP_Text nodeText;

    public TileInfoCollector tIC;

    private void Start()
    {
        //zoop();
        throwaway.text = $"You are on level {GameObject.FindObjectOfType<TileInfoCollector>().currentLevel}.<br>Nice!";

        tIC = GameObject.Find("CodeMan").GetComponent<TileInfoCollector>();

        HighlightFormerTiles();

        // Dont ask.
        GameObject.FindObjectOfType<GameData>().kink = true;
    }

    private void Update()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        int count = 0;
        for (int i = 0; i < nodes.Length; i++)
        {
            count += nodes[i].GetComponent<Draggable>().nodeStrength;
        }
        nodeText.text = $"Potential Reward:<br>${(count * 20) * 0.7f}k to ${(count * 20) * 1.5f}k";

        

    }

    /// <summary>
    /// Hide and disable one interface type when the other is active
    /// </summary>
    public void SwitchInterface()
    {
        tilesOrNodes = !tilesOrNodes;

        // Remember to disable one before enabling another.
        if (tilesOrNodes)
        {
            holders[0].SetActive(false);
            holders[1].SetActive(true);            
        }
        else
        {
            holders[1].SetActive(false);
            holders[0].SetActive(true);
        }        
    }

    /// <summary>
    /// Because Codeman becomes disconnected from the scene, buttons must use this function to save
    /// </summary>
    public void SaveAndContinue()
    {
        // Activate the node interface if disabled
        holders[0].SetActive(false);
        holders[1].SetActive(true);

        foreach (NodeHolderScript nhs in GameObject.FindObjectsOfType<NodeHolderScript>())
        {
            // do not continue the save until all nodes are used up
            if (nhs.nodeEnable)
            {
                return;
            }
        }
        
        // Record positions of the tiles
        GameObject.FindObjectOfType<TileInfoCollector>().SaveTiles();
        GameObject.FindObjectOfType<GameData>().SaveMoney();
        // Begin the minigame playlist
        GameObject.FindObjectOfType<GameData>().NextGame();
    }

    /// <summary>
    /// Used to highlight the tiles of the previous floor.
    /// </summary>
    void HighlightFormerTiles()
    {
        //Debug.Log("Starting Highlight");
        //TileInfoCollector tIC = GameObject.FindObjectOfType<TileInfoCollector>();

        if (tIC.currentLevel > 0)
        {
            bPosScript[] bPoses = GameObject.FindObjectsOfType<bPosScript>();

            // For each tile position saved from the previous floor
            for (int i = 0; i < tIC.nestedList[tIC.currentLevel - 1].tile.Count; i++) // Jesus what a line of code.
            {
                foreach (bPosScript b in bPoses)
                {
                    if (b.tileInfo.coordinates == tIC.nestedList[tIC.currentLevel - 1].tile[i].coordinates) // Once again man what a stroke of a code.
                    {
                        //Debug.Log($"Tile '{tIC.nestedList[tIC.currentLevel - 1].tile[i].tileName}' placed at {b.tileInfo.coordinates}");
                        GameObject g = Instantiate(Resources.Load($"Draggables/{tIC.nestedList[tIC.currentLevel - 1].tile[i].tileName}") as GameObject, b.transform.position, Quaternion.identity);
                        EnshadowTiles(b,g.GetComponent<Draggable>());
                        
                        g.layer = (int)LayerMask.NameToLayer("Ignore Raycast");
                        g.tag = "Untagged";
                        g.GetComponent<Renderer>().material.color = new Color(0,0,0,0.8f);  //new Color32(55, 232, 132, 100);
                        g.GetComponent<Renderer>().sortingOrder = 0;
                        Destroy(g.GetComponent<Collider2D>());
                        Destroy(g.GetComponent<Draggable>());
                    }
                }
            }
        }
        else
        {
            //Debug.Log(tIC.currentLevel);
            //Debug.Log("Error");
        }
    }


    void EnshadowTiles(bPosScript bPos, Draggable d)
    {
        bPos.shadowed = true;
        for (int i = 0; i < d.AdditionalPositions.Length; i++)
        {
            foreach (bPosScript b in GameObject.FindObjectsOfType<bPosScript>())
            {
                if (b.tileInfo.coordinates == bPos.tileInfo.coordinates + d.AdditionalPositions[i])
                {
                    b.shadowed = true;
                }
            }
        }        
    }


    /// <summary>
    /// For creating a series of minigames
    /// </summary>
    public void CreatePlaylist()
    {
        List<string> playlist = new List<string>();
        NodeHolderScript[] nhs = GameObject.FindObjectsOfType<NodeHolderScript>();
        foreach (NodeHolderScript zoop in nhs)
        {
            for (int i = 0; i < zoop.nodesMax; i++)
            {
                switch (zoop.nodeName)
                {
                    case "nodeVents":
                        playlist.Add("VentGame");
                        break;
                    case "nodePlumbing":
                        playlist.Add("PlumbingGame");
                        break;
                    case "nodeElectricity":
                        playlist.Add("ElecGame");
                        break;
                    default:
                        break;
                }

            }
        }

        // Randomize list
        for (int i = 0; i < playlist.Count; i++)
        {
            string temp = playlist[i];
            int randomIndex = Random.Range(i, playlist.Count);
            playlist[i] = playlist[randomIndex];
            playlist[randomIndex] = temp;
        }

        // Give list to the codeman as an array
        GameObject.FindObjectOfType<GameData>().playlist = playlist.ToArray();
    }

}
