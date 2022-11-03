using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VentGameScript : MonoBehaviour
{
    public int gridSize = 8;
    public int numberOfVents = 4;
    public float scale = 1;

    public GameObject ventPosition;
    public GameObject gameHolder;

    public List<VentObject> positions = new List<VentObject>();

    public GameObject ventNodeObject;
    public Color[] thingyBColors;

    public List<VentHeadScript> ventNodes = new List<VentHeadScript>();

    // Win Screen Stuff
    public TMP_Text winText;
    int stars = 5;
    bool stopTheGame = false;
    public float maxTime = 15;
    float currentTime = 15;
    //bool timerStopper = false;
    public Slider timerSlider;
    public TMP_Text starText;

    private void Start()
    {
        // Update game difficulty to suite the floor level
        if (GameObject.FindObjectOfType<TileInfoCollector>())
        {
            TileInfoCollector tic = GameObject.FindObjectOfType<TileInfoCollector>();

            if (numberOfVents + tic.currentLevel < 9) // There are 9 colors. Plus adding any more nodes would probably not be fun.
            {
                numberOfVents += tic.currentLevel;
            }
            else
            {
                numberOfVents = 8;
            }

            // Reduce time based on the current level
            if (maxTime - (tic.currentLevel * 0.5f) > 3)
            {
                maxTime = maxTime - (tic.currentLevel * 0.5f);
            }
            else
            {
                maxTime = 3;
            }
            currentTime = maxTime;
            timerSlider.maxValue = maxTime;


        }




        // Spawn coordinates
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                //GameObject g = Instantiate(thingyA, new Vector3(i-gridSize / 2,-j + gridSize / 2,0), Quaternion.identity);
                GameObject g = Instantiate(ventPosition, gameHolder.transform.position, Quaternion.identity);
                g.transform.parent = gameHolder.transform;
                g.transform.position = new Vector3((float)i - gridSize / 2, (float)-j + gridSize / 2, 0) * scale;
                g.transform.localScale = g.transform.localScale * scale;
                g.GetComponent<VentObject>().coord = new Vector2(i, j);
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
                        GameObject g = Instantiate(ventNodeObject, positions[rand].transform.position, Quaternion.identity);
                        g.transform.parent = positions[rand].transform;
                        g.transform.position += -Vector3.forward * 0.1f;
                        g.transform.localScale = g.transform.localScale * scale;
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
        if (!stopTheGame)
        {
            timerSlider.value = currentTime;
            if (currentTime <= 0)
            {
                if (stars > 1)
                {
                    currentTime = maxTime;
                    stars--;
                    starText.text = $"{stars} stars";
                }
            }
            else
            {
                currentTime -= Time.deltaTime;
            }


            CheckForWin();
        }
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
        winText.text = $"{i / 2} / {Mathf.Ceil((float)ventNodes.Count / 2 / 2)}";
        // Win condition
        Debug.Log(Mathf.Ceil((float)ventNodes.Count / 2 / 2));
        if (i >= ventNodes.Count / 2)
        {
            int moolah = GameData.Reward(stars);
            Debug.Log("Win");
            winText.text = $"{stars} Stars!<br><color=green>{moolah}k awarded!";

            // Reward finances
            GameData gd = GameObject.FindObjectOfType<GameData>();
            if (gd)
            {
                gd.expenditure += moolah;
                gd.starsOnLevel[gd.playlistOrder - 1] = stars;
            }
            // Stop recursion
            stopTheGame = true;
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

