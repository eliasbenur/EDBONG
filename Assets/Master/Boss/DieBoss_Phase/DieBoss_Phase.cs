﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class DieBoss_Phase : MonoBehaviour
{
    public float timer, timerTotBeforeDead, timerReturn;
    public float smoothTime = 2f;
    private Vector3 velocity;
    public GameObject camera;
    public GameObject canvas;

    bool checkedMonsters;

    public GameObject dieSmoke1, dieSmoke2;
    bool canSpawn;
    public float cooldown;

    public CinematicBars cinematicDie;

    public GameObject endOfLevelTrap;
    GameManager desactivateGodMode;

    private void Awake()
    {
        camera = Camera.main.gameObject;
        desactivateGodMode = Camera.main.GetComponent<GameManager>();
    }

    private void FixedUpdate()
    {
        cinematicDie.Show(200, 0.8f);
        var player = GameObject.Find("PlayerOne").GetComponent<Player_Movement>();
        var player2 = GameObject.Find("PlayerTwo").GetComponent<Player_Movement>();
        player.Stop_Moving();
        player2.Stop_Moving();


        timer += Time.deltaTime;
        BehaviorCamera();
        if (timer > timerTotBeforeDead && timer < timerReturn)
        {
            GetComponent<Animator>().Play("Die");
            StartCoroutine(Smoke_Dead());
        }

        if (timer > timerReturn)
        {
            camera.GetComponent<Camera_Focus>().enabled = true;
            canvas.SetActive(true);
            var desactivate = GameObject.FindGameObjectsWithTag("player");
            for (int i = 0; i < desactivate.Length; i++)
            {
                desactivate[i].GetComponent<Animator>().enabled = true;
                desactivate[i].GetComponent<Player_Movement>().enabled = true;
                desactivate[i].GetComponent<Player_Movement>().can_move = true;
            }
            cinematicDie.Hide(0.001f);
            player.can_move = true;
            player2.can_move = true;

            endOfLevelTrap.SetActive(true);
            desactivateGodMode.timerTotGodMode_p1 = desactivateGodMode.oldValueTimerGod;
            desactivateGodMode.timerTotGodMode_p2 = desactivateGodMode.oldValueTimerGod;
            desactivateGodMode.godMode_p1 = false;
            desactivateGodMode.godMode_p2 = false;
            desactivateGodMode.timerGodMode_p1 = 0;
            desactivateGodMode.timerGodMode_p2 = 0;

            AnalyticsEvent.Custom("Boss Completed", new Dictionary<string, object>
                    {
                        { "Scene", SceneManager.GetActiveScene().name },
                        { "Room" , transform.name }
                    });

   
            Destroy(this.gameObject);
        }
    }


    IEnumerator Smoke_Dead()
    {
        var position = Random.insideUnitSphere * 5 + transform.position;
        var smokeToSpawn = Random.Range(1, 3);
        var size = Random.Range(1, 6);
        switch (smokeToSpawn)
        {
            case 1:
                var smoke = Instantiate(dieSmoke1, position, Quaternion.identity);
                smoke.transform.localScale = new Vector2(size,size);
                break;

            case 2:
                smoke = Instantiate(dieSmoke2, position, Quaternion.identity);
                smoke.transform.localScale = new Vector2(size, size);
                break;
        }
        canSpawn = false;
        yield return new WaitForSeconds(cooldown);
        if(cooldown > 0.1)
            cooldown *= 0.9f;
        yield return null;
    }

    void BehaviorCamera()
    {
        canvas.SetActive(false);
        camera.GetComponent<Camera_Focus>().enabled = false;
        var desactivate = GameObject.FindGameObjectsWithTag("player");
        for (int i = 0; i < desactivate.Length; i++)
        {
            desactivate[i].GetComponent<Animator>().enabled = false;
            desactivate[i].GetComponent<Player_Movement>().enabled = false;
        }
        camera.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, new Vector3(transform.position.x, transform.position.y, camera.transform.position.z), ref velocity, smoothTime);
    }
}
