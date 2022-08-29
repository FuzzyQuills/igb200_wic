using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    TileInfoCollector tIC = null;

    private void Awake()
    {
        tIC = FindObjectOfType<TileInfoCollector>();
    }

    private void Start()
    {
        if (tIC)
        {
            for (int i = 0; i < tIC.tiles.Count; i++)
            {
                TileInfo t = tIC.tiles[i];
                if (t.occupied)
                {
                    GameObject g = Instantiate(Resources.Load($"BuildingModels/{t.tileName}") as GameObject, new Vector3(-t.coordinates.x,0.5f,-t.coordinates.y), Quaternion.identity);
                }
            }
        }
    }
}
