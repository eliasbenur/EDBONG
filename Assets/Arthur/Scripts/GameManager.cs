using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int money;

    [SerializeField]
    public List<GameObject> listItemDisplay;
    public bool KeyPressed;

    //public UParticleSystem_E_Modif RopePowerUp_Malus;
    public Player_Movement checkPlayer;
    public Player_Movement checkPlayer2;

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
        life = 20;

        Time.timeScale = 1;
        //gameOverCanvas.SetActive(false);

        //Display life on UI
        //displayLife = GameObject.Find("Slider").GetComponent<Slider>();

        money = 0;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
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
            checkPlayer.Vibrate_Control(0, 0);
            checkPlayer2.Vibrate_Control(0, 0);
            checkPlayer.enabled = false;
            checkPlayer2.enabled = false;
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
        }
    }

    public void Hit()
    {
        if (!godMode)
        {
            life -= 1;
            audio_ouff.Play();
            List<Transform> targets = GetComponent<Camera_Focus>().GetCameraTargets();
            for (int i = 0; i < targets.Count; i++)
            {
                if(targets[i].name == "PlayerOne")
                    targets[i].GetComponent<Player_Movement>().startBlinking = true;
                else if(targets[i].name == "PlayerTwo")
                    targets[i].GetComponent<Player_Movement>().startBlinking = true;
            }
        }

    }
}
