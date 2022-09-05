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

    private void Start()
    {
        zoop();
    }

    /// <summary>
    /// Hide one interface type when the other is active
    /// </summary>
    public void zoop()
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
}
