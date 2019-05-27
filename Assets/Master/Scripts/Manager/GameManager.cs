using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class GameManager : MonoBehaviour
{
    #region Properties
    [HideInInspector]public float max_Life;
    [HideInInspector]public int num_hits = 0;  

    [HideInInspector] public float life;
    [HideInInspector] public float shieldPoint;
    //Control of the life/ shield
    public GameObject gameOverCanvas;

    public GameObject shieldGameObject;
    private Slider shieldDisplay;

    List<Transform> targets;
    public List<God_Mode> players;
    public List<Player_Movement> players_Movement;
    #endregion

    public void Awake()
    {       
        life = 20;
        max_Life = life;
        shieldPoint = 0;

        //Display UI
        shieldDisplay = GameObject.Find("SliderShield").GetComponent<Slider>();   

        targets = GetComponent<Camera_Focus>().GetCameraTargets();
        for(int i = 0; i< targets.Count;i++)
            players.Add(targets[i].GetComponent<God_Mode>());

        for(int i = 0; i< targets.Count; i++)
            players_Movement.Add(targets[i].GetComponent<Player_Movement>());

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        SoundManager.Initialize();

        Time.timeScale = 1;
    }

    public void Start()
    {
        Application.targetFrameRate = 300;
        QualitySettings.vSyncCount = -1;

        //MUSIC
        //AkSoundEngine.PostEvent("play_dash", Camera.main.gameObject);
    }

    private void Update()
    {
        if (shieldPoint == 0)
            shieldGameObject.SetActive(false);
        else
        {
            shieldGameObject.SetActive(true);
            shieldDisplay.value = shieldPoint;
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
    
        for (int i = 0; i < players.Count; i++)
        {
            if(player == players[i].name)
                players[i].blinking_Player.startBlinking = true;
            else if(player == "PlayerUndefined" || player == "TwoOfThem")
                players[i].blinking_Player.startBlinking = true;
        } 
        Check_GameOver(pos, who_hit);
    }  
}
