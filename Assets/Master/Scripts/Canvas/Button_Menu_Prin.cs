using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_Menu_Prin : MonoBehaviour
{

    public void Quit()
    {
        Application.Quit();
    }

    public void NewGame_Button()
    {
        PlayerPrefs.SetFloat("player_X",0);
        PlayerPrefs.SetFloat("player_Y", 0);
        SceneManager.LoadScene("LD1", LoadSceneMode.Single);
    } 
}
