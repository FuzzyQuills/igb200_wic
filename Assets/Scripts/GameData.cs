using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// This is a general script for keeping save data between scenes. It goes in Code Man, meaning it will pass between scenes.
/// </summary>
public class GameData : MonoBehaviour
{
    public int money = 2000; // Each digit is $1k
    public int expenditure = 0; // A numerical collection of all money positive or negative in the gamespace

    public GameObject moneyUI_Canvas; // A unque canvas that holds the money UI elements
    TMP_Text moneyUI_Text;
    Slider moneyUI_Slider;

    Draggable[] dragsInScene; // For the blueprint phase

    public List<int> moneyChanges = new List<int>(); // Records of all times the amount of money has changed between scenes. For the inspection phase.

    public bool shit = true; // Temporary fix to a benign issue.


    private void Awake()
    {
        // Checks for duplicate scripts like this and destroys them.
        GameData[] gD = FindObjectsOfType<GameData>();
        if (gD.Length > 1)
        {
            if (moneyUI_Canvas)
            {
                Destroy(moneyUI_Canvas);
            }
            
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            if (moneyUI_Canvas)
            {
                DontDestroyOnLoad(moneyUI_Canvas);
            }
            else // If there's not a MoneyCanvas in this scene, then one is created.
            {
                GameObject g = Instantiate(Resources.Load("MoneyCanvas") as GameObject);
                g.name = "MoneyCanvas"; // Get rid of that stupid "(clone)" tag
                moneyUI_Canvas = g;
                DontDestroyOnLoad(moneyUI_Canvas);
            }
            
        }
        // Break down the Money UI elements into easily accessible variaables
        if (moneyUI_Canvas)
        {
            moneyUI_Text = moneyUI_Canvas.transform.GetComponentInChildren<TMP_Text>();
            moneyUI_Slider = moneyUI_Canvas.transform.GetComponentInChildren<Slider>();
        }        

    }


    private void Update()
    {
        UpdateMoneyUI();

        // Game over if your money reaches zero. Typically happens after the SaveMoney is called
        if (money < 0)
        {
            SceneManager.LoadScene("GameOver");
            money = 0;
        }
    }

    /// <summary>
    /// Changes the money UI to whatever the current money-to-expenditure for the scene is.
    /// </summary>
    public void UpdateMoneyUI()
    {

        if (shit)
        {
            // Get all the draggables in the scene and figure out their whole cost
            dragsInScene = GameObject.FindObjectsOfType<Draggable>();
            if (dragsInScene.Length > 0)
            {
                expenditure = 0;
                for (int i = 0; i < dragsInScene.Length; i++)
                {
                    expenditure += dragsInScene[i].price;
                }
            }
        }       


        if (expenditure == 0) // Only shows the money if there's not expenditure
        {
            moneyUI_Text.text =
                    $"${(money).ToString("n0")}k";
        }
        else if (expenditure <= 0) // Negative expenditure, show losses in red text
        {
            moneyUI_Text.text =
                $"<size=40><color=red>${expenditure.ToString("n0")}k</size></color><br>" +
                $"${(money + expenditure).ToString("n0")}k";
        }
        else if (expenditure >= 0) // Positive expenditure (oxymoron?), show gains in green text
        {
            moneyUI_Text.text =
                $"<size=40><color=green>${expenditure.ToString("n0")}k</size></color><br>" +
                $"${(money + expenditure).ToString("n0")}k";
        }        
        
        // Make the slider equal to the money + expenditure change.
        moneyUI_Slider.value = money + expenditure;

    }

    public void SaveMoney()
    {        

        // Add the expenditure of this scene to the MoneyChanges List
        moneyChanges.Add(expenditure);

        money += expenditure;

        shit = false;
        expenditure = 0;
    }


}
