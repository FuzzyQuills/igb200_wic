using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple script that holds a function for changing scenes.
/// </summary>
public class SceneScript : MonoBehaviour
{
    public bool debug = false;


    /// <summary>
    /// Changes the scene, obviously
    /// </summary>
    /// <param name="sceneName">Name of the scene to change to</param>
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Resets the game by removing all objects that store data / move between scenes
    /// </summary>
    public void ResetGame()
    {
        Destroy(GameObject.Find("CodeMan"));
        Destroy(GameObject.Find("MoneyCanvas"));
    }

    /// <summary>
    /// Triggers the save feature in the GameData script which should always be in CodeMan
    /// </summary>
    public void SaveGameData()
    {
        GameObject.Find("CodeMan")?.GetComponent<GameData>().SaveMoney();
    }

    private void Update()
    {
        if (debug)
        {
            // Reset scene
            if (Input.GetKeyDown(KeyCode.R))
            {                
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
