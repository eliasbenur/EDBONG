﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu_Manager : MonoBehaviour
{
    public GameObject menu;
    public GameObject cheatMode;
    public GameObject cheatModeButton;
    public Image endScreen;
    GameManager GameOver_Control;

    private void Start()
    {
        GameOver_Control = Camera.main.GetComponent<GameManager>();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameOver_Control.life > 0)
            {
                if (menu.activeSelf)
                {
                    menu.SetActive(false);
                    Time.timeScale = 1;
                }
                else
                {
                    menu.SetActive(true);
                    Time.timeScale = 0;
                }
            }
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Go_ToMainMenu()
    {
        AkSoundEngine.StopAll();
        SceneManager.LoadScene("Menu_Principal", LoadSceneMode.Single);
    }

    public void EndScreen()
    {
        StartCoroutine("Example");
    }

    public void Retry()
    {
        AkSoundEngine.StopAll();
        Load.load = true;
        AsyncOperation async = SceneManager.LoadSceneAsync("LD_Final", LoadSceneMode.Single);
    }

    IEnumerator Example()
    {
        yield return new WaitForSeconds(2);
        endScreen.gameObject.SetActive(true); 
        yield return new WaitForSeconds(10);
        AkSoundEngine.StopAll();
        SceneManager.LoadScene("Menu_Principal");
    }
}
