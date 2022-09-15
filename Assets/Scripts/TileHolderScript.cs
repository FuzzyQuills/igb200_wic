using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHolderScript : MonoBehaviour, IHolder
{
    SpriteRenderer sR;
    public string tileName;
    public Sprite[] tileSprites;
    public bool opened = false;

    private void Start()
    {
        sR = GetComponent<SpriteRenderer>();

        int rand = Random.Range(0,tileSprites.Length);
        sR.sprite = tileSprites[rand];

        tileName = sR.sprite.name;
    }
    
    public void UpdateOpened(bool Booley)
    {
        opened = Booley;
        if (opened)
        {
            sR.color = Color.gray;
        }
        else
        {
            sR.color = Color.white;
        }
    }

    private void OnMouseDown()
    {
        if (!opened)
        {
            //Debug.Log("A");
            GameObject g = Instantiate(Resources.Load($"Draggables/{tileName}") as GameObject, transform.position, Quaternion.identity);
            g.GetComponent<Draggable>().daddy = this;
            g.name = tileName;
            UpdateOpened(true);
        }
        
    }
}
