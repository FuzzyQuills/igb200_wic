using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileInfoCollector : MonoBehaviour
{   

    public int currentLevel = 0;

   
    //public List<List<TileInfo>> floorTiles = new List<List<TileInfo>>();
    public List<TileInfo> tiles;
    public List<testClass> nestedList = new List<testClass>();




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



    public void SaveTiles()
    {
        foreach (bPosScript b in FindObjectsOfType<bPosScript>())
        {
            tiles.Add(b.tileInfo);
        }

        //// Experimental. Saves multiple floors
        //floorTiles.Add(new List<TileInfo>());
        //foreach (bPosScript b in FindObjectsOfType<bPosScript>())
        //{
        //    floorTiles[currentLevel].Add(b.tileInfo);
        //}

        nestedList.Add(new testClass());
        foreach (bPosScript b in FindObjectsOfType<bPosScript>())
        {
            if (b.tileInfo.tiled && b.tileInfo.tileName != "Fill")
            {
                nestedList[currentLevel].tile.Add(b.tileInfo);
            }            
        }


        currentLevel++;
    }

    

    /// <summary>
    /// This is basically a way for me to see a list of lists in the inspector, which is a thing I cant do by default. Odd really.
    /// </summary>
    [System.Serializable]
    public class testClass
    {
        public List<TileInfo> tile = new List<TileInfo>();
    }
}
