using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Manager : MonoBehaviour
{
    public GameObject menu;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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

    public void Quit()
    {
        Application.Quit();
    }

    public void Go_ToMainMenu()
    {
        SceneManager.LoadScene("Menu_Principal");
    }
}
