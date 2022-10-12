using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    TileInfoCollector tIC = null;


    public GameObject buildingHolder;

    public GameObject cam;




    private void Awake()
    {
        tIC = FindObjectOfType<TileInfoCollector>();
    }

    private void Start()
    {
        cam.transform.position += Vector3.up * (0.7f * tIC.currentLevel);

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
                    if (t.tiled && t.tileName != "Fill")
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
        Vector3 target = buildingHolder.transform.position;
        if (tIC != null)
        {
            target = buildingHolder.transform.position + (Vector3.up * (0.35f + (0.7f * tIC.currentLevel)));
        }        
        cam.transform.LookAt(target);
        cam.transform.RotateAround(buildingHolder.transform.position, Vector3.up, 15 * Time.deltaTime);
        
        //buildingHolder.transform.Rotate(Vector3.up, 15 * Time.deltaTime);
    }
}
