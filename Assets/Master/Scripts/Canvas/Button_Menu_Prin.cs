using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Button_Menu_Prin : MonoBehaviour
{
    public Button LoadButton;

    public void Quit()
    {
        Application.Quit();
    }

    public void NewGame_Button()
    {
        SceneManager.LoadScene("LD1", LoadSceneMode.Single);    
    } 

    public void Interactable()
    {
        //if(SaveSystem.GetPersitentPath())

    }
}
