using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverSceneScript : MonoBehaviour
{
    public TMP_Text score;

    private void Start()
    {
        TileInfoCollector tIC = GameObject.Find("CodeMan").GetComponent<TileInfoCollector>();
        score.text = $"{tIC.currentLevel}<br>" +
            $"{tIC.perfectFloors}";

    }
}
