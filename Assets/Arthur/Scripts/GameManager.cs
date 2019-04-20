﻿using System.Collections;
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

    public string actualRoom;

    // GodMode P1
    public float timerGodMode_p1;
    public float timerTotGodMode_p1;
    public bool godMode_p1;
    // GodMode P2
    public float timerGodMode_p2;
    public float timerTotGodMode_p2;
    public bool godMode_p2;

    public AudioSource audio_ouff;

    public float life;
    public float shieldPoint;

    //Control of the life
    public GameObject gameOverCanvas;
    private Slider displayLife;
    public GameObject lifeDisplay;

    public GameObject shieldGameObject;
    private Slider shieldDisplay;

    //Items Shop
    public float regenLifeItem;
    public float maxLife;
    public AudioSource shopGuy;
    public AudioClip healSound, shieldSound, speedBoost;

    public float regenShieldItem;
    public float speedUp;

    public float oldValueTimerGod;

    public void Awake()
    {
        oldValueTimerGod = timerTotGodMode_p1;

        listItemDisplay.AddRange(GameObject.FindGameObjectsWithTag("Item"));

        life = 40;

        maxLife = life;

        shieldPoint = 0;

        shieldDisplay = GameObject.Find("SliderShield").GetComponent<Slider>();
        shieldGameObject.SetActive(false);
        Time.timeScale = 1;
        gameOverCanvas.SetActive(false);

        shieldPoint = 0;
        //Display life on UI
        displayLife = GameObject.Find("SliderHealth").GetComponent<Slider>();
        lifeDisplay.SetActive(true);
        displayLife.maxValue = life;
        money = 0;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        if (shieldPoint == 0)
        {
            shieldGameObject.SetActive(false);
        }
        else
        {
            shieldGameObject.SetActive(true);
            shieldDisplay.value = shieldPoint;
        }

        displayLife.value = life;

        for (int i = 0; i < listItemDisplay.Count; i++)
        {
            //  0/2 in case we have items with a price of two numbers
            var priceItem = listItemDisplay[i].GetComponentInChildren<Text>().text.Substring(0, 2);
            if (int.Parse(priceItem) > money)
            {
                listItemDisplay[i].GetComponentInChildren<Text>().color = Color.red;
            }
            else
            {
                listItemDisplay[i].GetComponentInChildren<Text>().color = Color.white;
            }
        }

        if (life <= 0)
        {
            gameOverCanvas.SetActive(true);
            lifeDisplay.SetActive(false);
            //checkPlayer.Vibrate_Control(0, 0);
            //checkPlayer.enabled = false;
            Time.timeScale = 0;

        }

        if (godMode_p1)
        {
            timerGodMode_p1 += Time.deltaTime;
            if (timerGodMode_p1 > timerTotGodMode_p1)
            {
                godMode_p1 = false;
                timerGodMode_p1 = 0;
            }
        }
        if (godMode_p2)
        {
            timerGodMode_p2 += Time.deltaTime;
            if (timerGodMode_p2 > timerTotGodMode_p2)
            {
                godMode_p2 = false;
                timerGodMode_p2 = 0;
            }
        }

        if (KeyPressed && Input.GetKeyDown(KeyCode.R)/* || Input.GetKey(KeyCode.Joystick2Button0)*/&& money >= int.Parse(checkPlayer.collisionItems.GetComponentInChildren<Text>().text.Substring(0, 2)))
        {
            for (int i = 0; i < listItemDisplay.Count; i++)
            {
                if (listItemDisplay[i] == checkPlayer.collisionItems.gameObject)
                {
                    switch (checkPlayer.collisionItems.gameObject.name)
                    {
                        case "HealthObject":
                            Debug.Log("Health Potion buy");

                            life += regenLifeItem;
                            if (life > maxLife)
                                life = maxLife;

                            money -= int.Parse(checkPlayer.collisionItems.GetComponentInChildren<Text>().text.Substring(0, 2));

                            shopGuy.clip = healSound;
                            shopGuy.Play();

                            KeyPressed = false;

                            listItemDisplay.RemoveAt(i);
                            Destroy(checkPlayer.collisionItems.gameObject);

                            break;

                        case "ShieldObject":
                            Debug.Log("Shield Potion buy");

                            shieldPoint += regenShieldItem;
                            if (shieldPoint > shieldDisplay.maxValue)
                                shieldPoint = shieldDisplay.maxValue;

                            money -= int.Parse(checkPlayer.collisionItems.GetComponentInChildren<Text>().text.Substring(0, 2));

                            shopGuy.clip = shieldSound;
                            shopGuy.Play();

                            listItemDisplay.RemoveAt(i);
                            Destroy(checkPlayer.collisionItems.gameObject);

                            break;

                        case "SpeedBoost":
                            Debug.Log("Speed Potion buy");

                            var players = GameObject.FindGameObjectsWithTag("player");
                            if (players != null)
                            {
                                foreach (GameObject player in players)
                                {
                                    player.GetComponent<Player_Movement>().speed = speedUp;
                                }
                            }

                            money -= int.Parse(checkPlayer.collisionItems.GetComponentInChildren<Text>().text.Substring(0, 2));

                            shopGuy.clip = speedBoost;
                            shopGuy.Play();

                            listItemDisplay.RemoveAt(i);
                            Destroy(checkPlayer.collisionItems.gameObject);
                            break;
                    }
                }
            }
        }
    }

    public void Hit_p1()
    {
        if (!godMode_p1)
        {
            if (shieldPoint <= 0)
                life -= 1;
            else
                shieldPoint -= 1;

            audio_ouff.Play();
            List<Transform> targets = GetComponent<Camera_Focus>().GetCameraTargets();
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].name == "PlayerOne")
                    targets[i].GetComponent<Player_Movement>().startBlinking = true;
                else if (targets[i].name == "PlayerTwo")
                    targets[i].GetComponent<Player_Movement>().startBlinking = true;
            }
        }

    }
    public void Hit_p2()
    {
        if (!godMode_p2)
        {
            if (shieldPoint <= 0)
                life -= 1;
            else
                shieldPoint -= 1;

            audio_ouff.Play();
            List<Transform> targets = GetComponent<Camera_Focus>().GetCameraTargets();
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].name == "PlayerOne")
                    targets[i].GetComponent<Player_Movement>().startBlinking = true;
                else if (targets[i].name == "PlayerTwo")
                    targets[i].GetComponent<Player_Movement>().startBlinking = true;
            }
        }

    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Go_ToMainMenu()
    {
        SceneManager.LoadScene("Menu_Principal");
    }
}
