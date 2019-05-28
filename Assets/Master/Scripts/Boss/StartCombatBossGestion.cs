using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCombatBossGestion : MonoBehaviour
{
    public float timer, timerTotStart;
    public GameObject boss;

    public GameObject canvas;

    public CinematicBars cinematicEnd;

    // Update is called once per frame
    void Update()
    {
        cinematicEnd.Hide(0.4f);
        timer += Time.deltaTime;
        if(timer > timerTotStart)
        {
            Camera.main.GetComponent<Camera_Focus>().enabled = true;
            var activate = GameObject.FindGameObjectsWithTag("player");
            for (int i = 0; i < activate.Length; i++)
            {
                activate[i].GetComponent<Animator>().enabled = true;
                //activate[i].GetComponent<Player_Movement>().enabled = true;
                activate[i].GetComponent<Player_Movement>().Allow_Moving();
            }
        }
        if(timer > timerTotStart +2.5)
        {
            boss.GetComponent<Boss>().enabled = true;
            canvas.SetActive(true);
            Destroy(this);
        }
    }
}
