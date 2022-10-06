using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VentGameScript : MonoBehaviour
{
    public int gridSize = 8;
    public int numberOfVents = 4;
    
    
    public GameObject thingyA;
    public GameObject thingyAHolder;

    public List<VentObject> positions = new List<VentObject>();

    public GameObject thingyB;
    public Color[] thingyBColors;
    
    public List<VentHeadScript> ventNodes = new List<VentHeadScript>();

    private void Start()
    {
        //// Color
        //thingyBColors = new Color[numberOfVents];
        //for (int i = 0; i < thingyBColors.Length; i++)
        //{
        //    thingyBColors[i] = Random.ColorHSV(0,1,0.8f,1,1,1);
        //    //thingyBColors[i] = Color.HSVToRGB((float)1/(numberOfVents * (i+1)),1,1);
        //}


        // Spawn coordinates
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                //GameObject g = Instantiate(thingyA, new Vector3(i-gridSize / 2,-j + gridSize / 2,0), Quaternion.identity);
                GameObject g = Instantiate(thingyA, thingyAHolder.transform);
                g.transform.position = new Vector3(i - gridSize / 2, -j + gridSize / 2, 0);
                g.GetComponent<VentObject>().coord = new Vector2(i,j);
                positions.Add(g.GetComponent<VentObject>());
            }
        }
        // Spawn the colored vents
        for (int i = 0; i < numberOfVents; i++)
        {
            Vector2 remember = -Vector2.one; // It works. Okay?
            for (int j = 0; j < 2; j++)
            {
                while (true)
                {
                    int rand = Random.Range(0, positions.Count - 1);
                    //Debug.Log(rand);
                    if (!positions[rand].vented && !near(positions[rand].coord, remember))
                    {
                        GameObject g = Instantiate(thingyB, positions[rand].transform.position - Vector3.forward, Quaternion.identity);
                        g.transform.parent = positions[rand].transform;
                        g.GetComponent<SpriteRenderer>().color = thingyBColors[i];
                        g.GetComponent<VentHeadScript>().identity = i;
                        g.GetComponent<VentHeadScript>().thisColor = thingyBColors[i];
                        g.GetComponent<VentHeadScript>().ventoObjecto = positions[rand];
                        ventNodes.Add(g.GetComponent<VentHeadScript>());
                        positions[rand].vented = true;
                        positions[rand].painted = true;
                        remember = positions[rand].coord;
                        break;
                    }
                }
            }
        }
    }

    private void Update()
    {
        CheckForWin();
    }

    public void CheckForWin()
    {
        int i = 0;
        foreach (VentHeadScript vhs in ventNodes)
        {
            if (vhs.connected)
            {
                i++;
            }
        }
        if (i >= ventNodes.Count / 2)
        {
            Debug.Log("Win");
        }        
    }

    /// <summary>
    /// This is used to get if a coordinate is horizontally or vertically adjacent from a point. Returns a bool
    /// </summary>
    /// <param name="thisVec"></param>
    /// <param name="targetVec"></param>
    /// <returns></returns>
    static public bool near(Vector2 thisVec, Vector2 targetVec)
    {
        if (thisVec.x == targetVec.x)
        {
            if (thisVec.y + 1 == targetVec.y || thisVec.y - 1 == targetVec.y)
            {
                //Debug.Log($"{thisVec} is too close to {targetVec}. Returning True");
                return true;
            }
        }
        else if (thisVec.y == targetVec.y)
        {
            if (thisVec.x + 1 == targetVec.x || thisVec.x - 1 == targetVec.x)
            {
                //Debug.Log($"{thisVec} is too close to {targetVec}. Returning True");
                return true;
            }
        }
        return false;
    }

}

