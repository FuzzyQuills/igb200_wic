using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SummmaryScript : MonoBehaviour
{
    public TMP_Text textbox;
    GameData gD;

    private void Start()
    {
        gD = GameObject.Find("CodeMan").GetComponent<GameData>();
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
        textbox.text += $"<color=white><size=160>Net Change:<br>{temp}k";
        yield return new WaitForSeconds(6);
        textbox.transform.GetComponent<CanvasGroup>().alpha = 0;
        yield return null;
    }
}
