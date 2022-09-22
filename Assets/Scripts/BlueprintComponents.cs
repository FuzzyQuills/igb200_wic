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


    private void Start()
    {
        //zoop();
        throwaway.text = $"You are on level {GameObject.FindObjectOfType<TileInfoCollector>().currentLevel}.<br>Nice!";

        HighlightFormerTiles();
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
    public void SaveAndContinue(string s)
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
        // Load the next scene
        SceneManager.LoadScene(s);
    }

    /// <summary>
    /// Used to highlight the tiles of the previous floor.
    /// </summary>
    void HighlightFormerTiles()
    {
        TileInfoCollector tIC = GameObject.FindObjectOfType<TileInfoCollector>();

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
                        GameObject g = Instantiate(Resources.Load($"Draggables/{tIC.nestedList[tIC.currentLevel - 1].tile[i].tileName}") as GameObject, b.transform);

                        g.GetComponent<Renderer>().material.color = new Color(0,0,0,0.3f);  //new Color32(55, 232, 132, 100);
                        g.GetComponent<Renderer>().sortingOrder = 0;
                        Destroy(g.GetComponent<Collider2D>());
                        Destroy(g.GetComponent<Draggable>());
                    }
                }
            }
        }
    }


}
