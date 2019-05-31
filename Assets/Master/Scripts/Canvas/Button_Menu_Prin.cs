using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Button_Menu_Prin : MonoBehaviour
{
    public Button LoadButton;

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
        SceneManager.LoadScene("LD1", LoadSceneMode.Single);    
    } 

    public void Load_Button()
    {
        Load.load = true;
        PlayerData data = SaveSystem.LoadPlayer();       
        SceneManager.LoadScene(data.level, LoadSceneMode.Single);        
    }

    public void Interactable()
    {
        if (!SaveSystem.GetPersitentPath())
            LoadButton.interactable = false;
        else
            LoadButton.interactable = true;
    }
}
