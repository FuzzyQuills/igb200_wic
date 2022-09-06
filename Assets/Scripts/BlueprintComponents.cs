using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    public void Save()
    {
        GameObject.FindObjectOfType<TileInfoCollector>().SaveTiles();
    }
}
