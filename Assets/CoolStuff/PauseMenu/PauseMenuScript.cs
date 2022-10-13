using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseMenuScript : MonoBehaviour
{
    public Color bgColor;
    bool paused = false;
    CanvasGroup cG;
    bool musicEnabled;
    public AudioMixer musicMixer;
    bool sfxEnabled;
    public AudioMixer sfxMixer;

    public Dialogue testD;

    int tutNumber = 0;

    public GameObject objectOfInterest = null;

    private void Awake()
    {
        cG = objectOfInterest.GetComponent<CanvasGroup>();
        Image bgImage = objectOfInterest.GetComponent<Image>();
        bgImage.color = bgColor;
    }

    // Legacy Code. Ignore
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {            
    //        if (!paused)
    //        {
    //            OpenMenu();
    //        }
    //        else
    //        {
    //            CloseMenu();
    //        }            
    //    }        
    //}
    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    void OpenMenu()
    {
        objectOfInterest.SetActive(true);
        paused = true;
        cG.alpha = 1;
        Time.timeScale = 0;
    }
    public void CloseMenu()
    {
        objectOfInterest.SetActive(false);
        paused = false;
        cG.alpha = 0;
        Time.timeScale = 1;
    }

    public void SetMusicLevel(float sliderVal)
    {
        musicMixer.SetFloat("Vol", Mathf.Log10(sliderVal) * 20);
    }
    public void SetSFXLevel(float sliderVal)
    {
        sfxMixer.SetFloat("Vol", Mathf.Log10(sliderVal) * 20);
    }


    public void PauseButton()
    {
        if (!paused)
        {
            OpenMenu();
            StartCoroutine(testD.InstantTextSingle(tutNumber));
        }
        else
        {
            CloseMenu();
            testD.CloseBox();
        }
    }

    public void ButtonNext()
    {
        if (tutNumber + 1 >= testD.dialogue.Length)
        {
            tutNumber = 0;
        }
        else
        {
            tutNumber++;
        }        
        StartCoroutine(testD.InstantTextSingle(tutNumber));
    }
}
