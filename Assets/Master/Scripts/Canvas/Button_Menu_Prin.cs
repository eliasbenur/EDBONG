using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_Menu_Prin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void NewGame_Button()
    {
        SceneManager.LoadScene("LD1", LoadSceneMode.Single);
    }

    public void Proto_Button()
    {
        SceneManager.LoadScene("Proto", LoadSceneMode.Single);
    }
}
