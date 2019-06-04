using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Button_Menu_Prin : MonoBehaviour
{
    public Button LoadButton;
    public GameObject loadingScreen;

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
        StartCoroutine(LoadNewScene());
        loadingScreen.SetActive(true);
    } 

    IEnumerator LoadNewScene()
    {
        yield return new WaitForSeconds(1);
        AsyncOperation async = SceneManager.LoadSceneAsync("LD_Final");
        while (!async.isDone)
        {
            yield return null;
        }
    }

    public void Load_Button()
    {
        StartCoroutine(LoadNewScene());
        loadingScreen.SetActive(true);

        Load.load = true;
        PlayerData data = SaveSystem.LoadPlayer();
        AsyncOperation async = SceneManager.LoadSceneAsync(data.level);      
    }

    public void Interactable()
    {
        if (!SaveSystem.GetPersitentPath())
            LoadButton.interactable = false;
        else
            LoadButton.interactable = true;
    }
}
