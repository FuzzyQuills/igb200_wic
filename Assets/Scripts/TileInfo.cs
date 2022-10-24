using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for holding information on what is held within a tile
/// </summary>

[System.Serializable]
public class TileInfo
{
    public Vector2 coordinates;
    
    public bool tiled = false;
    public string tileName;

    public bool noded = false;
    public string nodeName;
}
