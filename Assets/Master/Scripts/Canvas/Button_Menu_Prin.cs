using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Button_Menu_Prin : MonoBehaviour
{
    public Button LoadButton;
    public GameObject loadingScreen;
    public GameObject Boss;

    public void Start()
    {
        Interactable();    
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void NewGame_Button()
    {       
        loadingScreen.SetActive(true);
        Load.cinematic = true;
        Boss.GetComponent<Animator>().enabled = true;
        AsyncOperation async = SceneManager.LoadSceneAsync("LD_Final", LoadSceneMode.Single);
    } 

    public void Load_Button()
    {
        loadingScreen.SetActive(true);
        Load.load = true;
        Boss.GetComponent<Animator>().enabled = true;
        AsyncOperation async = SceneManager.LoadSceneAsync("LD_Final", LoadSceneMode.Single);
    }

    public void Interactable()
    {
        if (!SaveSystem.GetPersitentPath())
            LoadButton.interactable = false;
        else
            LoadButton.interactable = true;
    }
}
