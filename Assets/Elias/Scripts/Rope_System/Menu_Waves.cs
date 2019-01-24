using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Waves : MonoBehaviour
{
    public GameObject Menu_Esc;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Menu_Esc.activeSelf)
            {
                Menu_Esc.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                Menu_Esc.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

    public void GoTo_MainMenu()
    {
        SceneManager.LoadScene("Menu_Principal", LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
