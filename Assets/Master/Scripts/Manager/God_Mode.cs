using System.Collections.Generic;
using UnityEngine;

public class God_Mode : MonoBehaviour
{
    #region Properties
    [HideInInspector]public float timerGodMode;
    public float timerTotGodMode;
    public bool godMode;

    [HideInInspector] public float oldValueTimerGod;
    GameManager hit_check;

    //Use the list targets on the camera, to get the players
    List<Transform> targets;
    [HideInInspector]public Player_Movement blinking_Player;
    #endregion

    private void Awake()
    {
        oldValueTimerGod = timerTotGodMode;
        hit_check = Camera.main.GetComponent<GameManager>();
        targets = Camera.main.GetComponent<Camera_Focus>().GetCameraTargets();
        blinking_Player = GetComponent<Player_Movement>();
    }

    void Update()
    {
        if (godMode)
        {
            timerGodMode += Time.deltaTime;
            if (timerGodMode > timerTotGodMode)
            {
                godMode = false;
                timerGodMode = 0;
            }
        }
    }

    public void Hit_verification(string player, Vector3 pos, string who_hit)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (player == "PlayerOne" && !godMode)
            {
                hit_check.Hit(player, pos, who_hit);
                godMode = true;
            }
            else if (player == "PlayerTwo" && !godMode)
            {
                hit_check.Hit(player, pos, who_hit);
                godMode = true;
            }
            else if (player == "PlayerUndefined" || player == "TwoOfThem")
            {        
                for(int j =0;j < hit_check.players.Count;j++)
                {
                    if (!hit_check.players[j].godMode)
                        hit_check.players[j].godMode = true;
                    else
                        break;
                }
                hit_check.Hit(player, pos, who_hit);
                blinking_Player.startBlinking = true;
            }
        }
    }
}
