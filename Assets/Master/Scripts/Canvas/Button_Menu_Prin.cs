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
        SceneManager.LoadScene("LD1", LoadSceneMode.Single);    
    } 
}
