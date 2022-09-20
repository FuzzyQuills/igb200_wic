using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NodeHolderScript : MonoBehaviour, IHolder
{
    public string nodeName = "nodeElectricity";

    public int nodesMax = 0;
    public int nodesPlaced = 0;
    public TMP_Text countText;

    public bool nodeEnable = true;

    public void UpdateOpened(bool Booley)
    {
        nodesPlaced--;
        nodeEnable = true;
    }

    void Start()
    {
        if (nodesMax == 0)
        {
            nodeEnable = false;
        }
    }

    private void OnMouseDown()
    {
        if (nodeEnable)
        {
            Debug.Log("B");
            GameObject g = Instantiate(Resources.Load($"Draggables/{nodeName}") as GameObject, transform.position, Quaternion.identity);
            g.GetComponent<Draggable>().daddy = this;
            g.name = nodeName;
            //UpdateOpened(true);
            nodesPlaced++;
            if (nodesMax == nodesPlaced) // Disable this function if all tiles are placed
            {
                nodeEnable = false;
            }
        }
    }

    
    void Update()
    {
        if (nodesMax == nodesPlaced)
        {
            countText.text = "Good Work";
        }
        else
        {
            countText.text = $"+{nodesMax - nodesPlaced} required";
        }
    }
}
