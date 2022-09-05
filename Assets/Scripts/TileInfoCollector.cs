using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfoCollector : MonoBehaviour
{
    private void Awake()
    {
        // Checks for duplicate scripts like this and destroys them.
        TileInfoCollector[] tIC = FindObjectsOfType<TileInfoCollector>();
        if (tIC.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }        
    }

    public List<TileInfo> tiles;


    public void SaveTiles()
    {
        foreach (bPosScript b in FindObjectsOfType<bPosScript>())
        {
            tiles.Add(b.tileInfo);
        }
    }
}
