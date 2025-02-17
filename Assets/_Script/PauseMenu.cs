﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    public GameObject settingsUI;
     public GameObject alertUI;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();


            }
            else
            {

                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        alertUI.SetActive(true);
        Time.timeScale=1;
        isPaused=false;

    }

    public void showSettings()
    {
        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(true);

    }
    public void hideSettings()
    {
        pauseMenuUI.SetActive(true);
        settingsUI.SetActive(false);

    }
   public void Pause()
    {
        pauseMenuUI.SetActive(true);
        alertUI.SetActive(false);
        Time.timeScale=0;
        isPaused=true;

        

    }
    public void quitToMenu(){
        Time.timeScale=1;
        SceneManager.LoadScene(0);
    }
}

