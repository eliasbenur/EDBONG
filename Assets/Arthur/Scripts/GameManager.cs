using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Kilt.EasyRopes2D;

public class GameManager : MonoBehaviour
{
    public int money;

    [SerializeField]
    public List<GameObject> listItemDisplay;
    public bool KeyPressed;

    //public UParticleSystem_E_Modif RopePowerUp_Malus;
    public TackRope RopePowerUp_Malus;
    public Player_Movement checkPlayer;

    public GameObject gameOverCanvas;

    public float life;

    public string actualRoom;

    //Control of the godMode
    public float timerGodMode;
    public float timerTotGodMode;
    public bool godMode;

    private Slider displayLife;

    public AudioSource audio_ouff;

    public void Awake()
    {
        //listItemDisplay.AddRange(GameObject.FindGameObjectsWithTag("Item"));


        //Variable en dur !! Warning
        life = 10;

        Time.timeScale = 1;
        //gameOverCanvas.SetActive(false);

        //Display life on UI
        //displayLife = GameObject.Find("Slider").GetComponent<Slider>();

        money = 0;
    }

    private void Update()
    {
        //displayLife.value = life;

        for (int i = 0; i < listItemDisplay.Count; i++)
        {
            //  0/2 in case we have items with a price of two numbers
            var priceItem = listItemDisplay[i].GetComponentInChildren<TextMeshProUGUI>().text.Substring(0, 2);
            if (System.Convert.ToInt32(priceItem) > money)
            {
                listItemDisplay[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            }
            else
            {
                listItemDisplay[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
        }

        if (life <= 0)
        {
            //SceneManager.LoadScene("ProtoJuicy");
            //gameOverCanvas.SetActive(true);
            Time.timeScale = 0;
        }

        if (godMode)
        {
            timerGodMode += Time.deltaTime;
            if (timerGodMode > timerTotGodMode)
            {
                godMode = false;
                timerGodMode = 0;
            }
        }

        if (KeyPressed && Input.GetKey(KeyCode.Joystick1Button1) /*|| Input.GetKey(KeyCode.Joystick2Button0)*/ && money >= System.Convert.ToInt32(checkPlayer.collisionItems.GetComponentInChildren<TextMeshProUGUI>().text.Substring(0, 2)))
        {
            money -= System.Convert.ToInt32(checkPlayer.collisionItems.GetComponentInChildren<TextMeshProUGUI>().text.Substring(0, 2));
            for (int i = 0; i < listItemDisplay.Count; i++)
            {
                if (listItemDisplay[i] == checkPlayer.collisionItems.gameObject)
                {
                    Debug.Log("Object Found");
                    if (listItemDisplay[i].name != "Reset")
                    {
                        listItemDisplay.RemoveAt(i);
                        Destroy(checkPlayer.collisionItems.gameObject);
                    }
                }
            }

            switch (checkPlayer.collisionItems.gameObject.name)
            {
                case "PlusLengthRope":
                    Debug.Log("PlusRope");
                    RopePowerUp_Malus.AmountOfNodes = 60;
                    //RopePowerUp_Malus.CableLengthDesired = 50;
                    break;
                case "HeavyObject":
                    Debug.Log("Heavy");
                    RopePowerUp_Malus.NodeMass = 10;
                    //RopePowerUp_Malus.gravity = Vector3.zero;
                    break;
                case "MinusLengthRope":
                    Debug.Log("MinusRope");
                    RopePowerUp_Malus.AmountOfNodes = 20;
                    //RopePowerUp_Malus.CableLengthDesired = 10;
                    break;
                case "Node_Linear_Drag_Rope":
                    Debug.Log("LinearDrag");
                    RopePowerUp_Malus.NodeLinearDrag = 1;
                    //RopePowerUp_Malus.CableLengthDesired = 10;
                    break;
                case "Node_Angular_Drag_Rope":
                    Debug.Log("Angular_Drag");
                    RopePowerUp_Malus.NodeAngularDrag = 1;
                    //RopePowerUp_Malus.CableLengthDesired = 10;
                    break;
                case "Node_Gravity_Scale_Rope":
                    Debug.Log("Gravity_Scale");
                    RopePowerUp_Malus.NodeGravityScale = 1;
                    //RopePowerUp_Malus.CableLengthDesired = 10;
                    break;
                case "Reset":
                    Debug.Log("reset");
                    RopePowerUp_Malus.NodeGravityScale = 0;
                    RopePowerUp_Malus.NodeAngularDrag = 0.25f;
                    RopePowerUp_Malus.NodeLinearDrag = 0.01f;
                    RopePowerUp_Malus.AmountOfNodes = 30;
                    RopePowerUp_Malus.NodeMass = 1.5f;
                    //RopePowerUp_Malus.CableLengthDesired = 10;
                    break;
                default:
                    break;
            }
        }
    }

    public void Hit()
    {
        if (!godMode)
        {
            life -= 1;
            audio_ouff.Play();
            for (int i = 0; i < GetComponent<Camera_Focus>().targets.Count; i++)
            {
                if(GetComponent<Camera_Focus>().targets[i].name == "PlayerOne")
                    GetComponent<Camera_Focus>().targets[i].GetComponent<Player_Movement>().startBlinking = true;
                else if(GetComponent<Camera_Focus>().targets[i].name == "PlayerTwo")
                    GetComponent<Camera_Focus>().targets[i].GetComponent<Player2_Movement>().startBlinking = true;
            }
        }

    }
}
