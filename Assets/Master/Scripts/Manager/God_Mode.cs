using System.Collections;
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

                blinking_Player.gameObject.GetComponent<Animator>().SetBool("hit", true);
                StartCoroutine("Delay_hit_animation");
            }
            else if (player == "PlayerTwo" && !godMode)
            {
                hit_check.Hit(player, pos, who_hit);
                godMode = true;

                blinking_Player.gameObject.GetComponent<Animator>().SetBool("hit", true);
                StartCoroutine("Delay_hit_animation");
            }
            else if (player == "PlayerUndefined" || player == "TwoOfThem")
            {
                if (!hit_check.players[0].godMode && !hit_check.players[1].godMode)
                {
                    hit_check.players[0].godMode = true;
                    hit_check.players[1].godMode = true;
                    hit_check.Hit(player, pos, who_hit);

                    blinking_Player.gameObject.GetComponent<Animator>().SetBool("hit", true);
                    StartCoroutine("Delay_hit_animation");
                }        
            }
        }
    }

    IEnumerator Delay_hit_animation()
    {
        yield return new WaitForSeconds(0.2f);
        blinking_Player.gameObject.GetComponent<Animator>().SetBool("hit", false);
    }
}
