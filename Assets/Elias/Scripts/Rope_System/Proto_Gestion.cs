using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Proto_Gestion : MonoBehaviour
{

    public GameObject Prefab_Monsters;
    public int room_act;
    public bool ia_room1, ia_room2, ia_room3, ia_room4, ia_room5;
    public Toggle togle;
    public GameObject Menu_Esc;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Joystick1Button6))
        {
            Respawn_Enemies();
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            switch (room_act)
            {
                case 1:
                    if (!ia_room1)
                    {
                        Active_IA();
                        ia_room1 = true;
                    }
                    else
                    {
                        Desactive_IA();
                        ia_room1 = false;
                    }
                    break;
                case 2:
                    if (!ia_room2)
                    {
                        Active_IA();
                        ia_room2 = true;
                    }
                    else
                    {
                        Desactive_IA();
                        ia_room2 = false;
                    }
                    break;
                case 3:
                    if (!ia_room3)
                    {
                        Active_IA();
                        ia_room3 = true;
                    }
                    else
                    {
                        Desactive_IA();
                        ia_room3 = false;
                    }
                    break;
                case 4:
                    if (!ia_room4)
                    {
                        Active_IA();
                        ia_room4 = true;
                    }
                    else
                    {
                        Desactive_IA();
                        ia_room4 = false;
                    }
                    break;
                case 5:
                    if (!ia_room5)
                    {
                        Active_IA();
                        ia_room5 = true;
                    }
                    else
                    {
                        Desactive_IA();
                        ia_room5 = false;
                    }
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Menu_Esc.activeSelf){
                Menu_Esc.SetActive(false);
                Time.timeScale = 1;
            }
            else{
                Menu_Esc.SetActive(true);
                Time.timeScale = 0;
            }
        }

        Gestion_Toggle();
    }

    public void GoTo_MainMenu()
    {
        SceneManager.LoadScene("Menu_Principal", LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Gestion_Toggle()
    {
        switch (room_act)
        {
            case 1:
                if (ia_room1)
                {
                    togle.isOn = true;
                }
                else
                {
                    togle.isOn = false;
                }
                break;
            case 2:
                if (ia_room2)
                {
                    togle.isOn = true;
                }
                else
                {
                    togle.isOn = false;
                }
                break;
            case 3:
                if (ia_room3)
                {
                    togle.isOn = true;
                }
                else
                {
                    togle.isOn = false;
                }
                break;
            case 4:
                if (ia_room4)
                {
                    togle.isOn = true;
                }
                else
                {
                    togle.isOn = false;
                }
                break;
            case 5:
                if (ia_room5)
                {
                    togle.isOn = true;
                }
                else
                {
                    togle.isOn = false;
                }
                break;
        }
    }

    public void Respawn_Enemies()
    {
        Destroy(GameObject.Find("Monsters_Proto(Clone)"));
        Instantiate(Prefab_Monsters, Vector3.zero, Quaternion.identity);
    }

    public void Active_IA()
    {
        Transform[] allChildren = GameObject.Find("Monsters_Proto(Clone)").GetComponentsInChildren<Transform>();
        foreach (Transform monster in allChildren)
        {
            if (monster.tag == "monster")
            {
                if (monster.name == "Monster_coupe" && room_act == 1)
                {
                    monster.GetComponent<basicAI_E>().enemySpeed = 2;
                }else if (monster.name == "Monster_sourround" && room_act == 2)
                {
                    monster.GetComponent<basicAI2_E>().enemySpeed = 2;
                }else if (monster.name == "Monster_surround_dash" && room_act == 3)
                {
                    monster.GetComponent<basicAI2_E_dash>().enemySpeed = 2;
                }else if (monster.name == "Monster_piege" && room_act == 4)
                {
                    monster.GetComponent<toutbete>().enemySpeed = 0.2f;
                }else if (monster.name == "Monster_collant" && room_act == 5)
                {
                    monster.GetComponent<AI_collant>().enemySpeed = 0.2f;
                }
            }
        }
    }

    public void Desactive_IA()
    {
        Transform[] allChildren = GameObject.Find("Monsters_Proto(Clone)").GetComponentsInChildren<Transform>();
        foreach (Transform monster in allChildren)
        {
            if (monster.tag == "monster")
            {
                if (monster.name == "Monster_coupe" && room_act == 1)
                {
                    monster.GetComponent<basicAI_E>().enemySpeed = 0;
                }
                else if (monster.name == "Monster_sourround" && room_act == 2)
                {
                    monster.GetComponent<basicAI2_E>().enemySpeed = 0;
                }
                else if (monster.name == "Monster_surround_dash" && room_act == 3)
                {
                    monster.GetComponent<basicAI2_E_dash>().enemySpeed = 0;
                }
                else if (monster.name == "Monster_piege" && room_act == 4)
                {
                    monster.GetComponent<toutbete>().enemySpeed = 0;
                }
                else if (monster.name == "Monster_collant" && room_act == 5)
                {
                    monster.GetComponent<AI_collant>().enemySpeed = 0;
                }
            }
        }
    }
}
