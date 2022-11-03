using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class SummmaryScript : MonoBehaviour
{
    public TMP_Text textbox;
    GameData gD;
    TileInfoCollector tic;

    private void Start()
    {
        gD = GameObject.Find("CodeMan").GetComponent<GameData>();
        tic = GameObject.Find("CodeMan").GetComponent<TileInfoCollector>();
        StartCoroutine(displayGainz());
    }


    IEnumerator displayGainz()
    {
        textbox.text = "";
        int temp = 0;
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < gD.moneyChanges.Count; i++)
        {
            if (i == 0)
            {
                textbox.text += $"<color=white>Floor Cost:";
            }
            else
            {
                textbox.text += $"<color=white>{gD.playlist[i - 1].Substring(0,gD.playlist[i - 1].Length - 4)} gains:";
            }
           
            switch (gD.moneyChanges[i])
            {
                case < 0:
                    textbox.text += $"<color=red>{gD.moneyChanges[i]}k<br>";
                    break;
                case 0:
                    textbox.text += $"<color=white>{gD.moneyChanges[i]}k<br>";
                    break;
                case > 0:
                    textbox.text += $"<color=green>{gD.moneyChanges[i]}k<br>";
                    break;
            }
            temp += gD.moneyChanges[i];
            yield return new WaitForSeconds(0.4f);
        }
        yield return new WaitForSeconds(0.2f);
        string s = textbox.text;
        textbox.text = s + $"<color=white><size=140>Change:{temp}k<br>" +
                           $"Funding:{gD.money - temp}k";
        yield return new WaitForSeconds(1);
        for (int i = 10; i > 0; i--)
        {
            textbox.text = s + $"<color=white><size=140>Change:{temp}k<br>" +
                               $"Funding:{gD.money - temp + (temp/i)}k";
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(3);
        if (gD.money <= 0)
        {
            Destroy(GameObject.Find("MoneyCanvas"));
            //Destroy(GameObject.Find("CodeMan"));
            SceneManager.LoadScene("GameOver");
        }
        else
        {
            int a = 0;
            if (tic.nestedList[tic.currentLevel - 1].perfect)
            {
                for (int i = 0; i < gD.starsOnLevel.Length; i++)
                {
                    a = 0;
                    textbox.text = "<size=160>Nice Work!";
                    if (gD.starsOnLevel[i] < 5)
                    {
                        break;
                    }
                    textbox.text = "<size=160>Perfect Floor!";
                    a = 1;
                }
            }
            else
            {
                textbox.text = "<size=160>Nice Work!";
            }
            tic.perfectFloors += a; // Cringe code
        }
        yield return null;
    }

}
