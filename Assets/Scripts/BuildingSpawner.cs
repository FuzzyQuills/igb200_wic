using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    TileInfoCollector tIC = null;


    public GameObject buildingHolder;

    private void Awake()
    {
        tIC = FindObjectOfType<TileInfoCollector>();
    }

    private void Start()
    {
        if (tIC)
        {
            //for (int i = 0; i < tIC.tiles.Count; i++)
            //{
            //    TileInfo t = tIC.tiles[i];
            //    if (t.tiled)
            //    {
            //        GameObject g = Instantiate(Resources.Load($"BuildingModels/{t.tileName}") as GameObject, new Vector3(-t.coordinates.x,0.5f,-t.coordinates.y), Quaternion.identity);
            //    }
            //}
            for (int i = 0; i < tIC.nestedList.Count; i++)
            {
                for (int j = 0; j < tIC.nestedList[i].tile.Count; j++)
                {
                    TileInfo t = tIC.nestedList[i].tile[j];
                    if (t.tiled)
                    {
                        GameObject g = Instantiate(Resources.Load($"BuildingModels/{t.tileName}") as GameObject, new Vector3(t.coordinates.x, (i * 0.7f), -t.coordinates.y), Quaternion.identity);
                        g.transform.parent = buildingHolder.transform;
                    }
                }
            }
        }
    }

    private void Update()
    {
        buildingHolder.transform.Rotate(Vector3.up, 15 * Time.deltaTime);
    }
}
