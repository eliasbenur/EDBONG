﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Properties
    [HideInInspector]public float max_Life;
    [HideInInspector]public int num_hits = 0;  

    public float life;
    public float shieldPoint;
    //Control of the life/ shield
    public GameObject gameOverCanvas;

    //Live and Shield UI
    //public GameObject shieldGameObject;
    private Image liveDisplay;
    private Image liveDisplay_back;
    private bool update_lifeback = false;
    private GameObject shieldDisplay;
    public GameObject shield_UI;

    List<Transform> targets;
    public List<God_Mode> players;
    public List<Player_Movement> players_Movement;

    //Use for the save
    public Rope_System rope;
    [HideInInspector] public Vector3 ropeSystem_position;
    [HideInInspector] public string active_Scene;
    #endregion

    public void Awake()
    {       
        life = 20;
        max_Life = life;

        //Display UI
        liveDisplay = GameObject.Find("Life_Bar_Filled").GetComponent<Image>();
        liveDisplay_back = GameObject.Find("Life_Bar_Back").GetComponent<Image>();
        shieldDisplay = GameObject.Find("Shields_UI");

        targets = GetComponent<Camera_Focus>().GetCameraTargets();
        for(int i = 0; i< targets.Count;i++)
            players.Add(targets[i].GetComponent<God_Mode>());

        for(int i = 0; i< targets.Count; i++)
            players_Movement.Add(targets[i].GetComponent<Player_Movement>());

        Time.timeScale = 1;
    }

    public void Start()
    {
        Application.targetFrameRate = 300;

        //MUSIC
        AkSoundEngine.PostEvent("play_amb", Camera.main.gameObject);

        Update_shieldDisyplay();
        Update_liveDisplay();

        active_Scene = SceneManager.GetActiveScene().name;
    }

    public void Update()
    {
        if (update_lifeback)
        {
            if (liveDisplay.fillAmount < liveDisplay_back.fillAmount)
            {
                liveDisplay_back.fillAmount = liveDisplay_back.fillAmount - 0.05f * Time.deltaTime;
            }
            else
            {
                update_lifeback = false;
            }
        }
    }

    public void Update_liveDisplay()
    {
        liveDisplay.fillAmount = life / max_Life;
        StartCoroutine("update_back_liveDisplay");
    }

    IEnumerator update_back_liveDisplay()
    {
        yield return new WaitForSeconds(2);
        update_lifeback = true;
    }

    public void Update_shieldDisyplay()
    {
        if (shieldPoint == 0)
        {
            for (int x  = 0; x < shieldDisplay.transform.childCount; x++)
            {
                Destroy(shieldDisplay.transform.GetChild(x).gameObject);
            }
        }
        else
        {
            float dif_shield = shieldPoint - shieldDisplay.transform.childCount;
            Debug.Log(dif_shield);
            if (dif_shield > 0)
            {
                for (int x = 0; x < dif_shield; x++)
                {
                    // Instance
                    GameObject ref_obj = Instantiate(shield_UI, shieldDisplay.transform.position + new Vector3(70 * shieldDisplay.transform.childCount, 0,0), Quaternion.identity);
                    ref_obj.transform.parent = shieldDisplay.transform;
                }
            }
            else
            {
                for (int x = 0; x < (dif_shield * -1); x++)
                {
                    // Destroy
                    Destroy(shieldDisplay.transform.GetChild(shieldDisplay.transform.childCount - 1 - x).gameObject);
                }
            }
        }
    }

    public void Check_GameOver(Vector3 pos, string who_hit)
    {
        if (life <= 0)
        {
            AnalyticsEvent.Custom("Death", new Dictionary<string, object>
            {
                { "pos", pos },
                { "who_hit" , who_hit }
            });
            AkSoundEngine.PostEvent("play_death", Camera.main.gameObject);
            gameOverCanvas.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void Hit(string player, Vector3 pos, string who_hit)
    {
        AnalyticsEvent.Custom("Hit", new Dictionary<string, object>
        {
            { "player_hit", player },
            { "pos", pos },
            { "who_hit" , who_hit }
        });
        num_hits++;

        AkSoundEngine.PostEvent("play_playerhit", Camera.main.gameObject);

        if (shieldPoint <= 0)
            life -= 1;
        else
            shieldPoint -= 1;

        Update_liveDisplay();
        Update_shieldDisyplay();


        for (int i = 0; i < players.Count; i++)
        {
            if(player == players[i].name)
                players[i].blinking_Player.startBlinking = true;
            else if(player == "PlayerUndefined" || player == "TwoOfThem")
                players[i].blinking_Player.startBlinking = true;
        } 
        Check_GameOver(pos, who_hit);
    }

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        active_Scene = data.level;
        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];

        transform.position = position;
        rope.transform.position = position;

        rope.get_points()[0].transform.position = position;
        rope.get_points()[rope.NumPoints - 1].transform.position = position;

        Vector3 Delta = rope.get_points()[rope.NumPoints - 1].transform.position - rope.get_points()[0].transform.position;
        for (int ParticleIndex = 0; ParticleIndex < rope.NumPoints; ParticleIndex++)
        {
            float Alpha = (float)ParticleIndex / (float)(rope.NumPoints - 1);
            Vector3 InitializePosition = rope.get_points()[0].transform.position + (Alpha * Delta);
            rope.get_points()[ParticleIndex].transform.position = InitializePosition;
        }
    }
}
