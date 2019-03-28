﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviorEnterBossRoom : MonoBehaviour
{
    bool detected;
    public float smoothTime = 2f;
    private Vector3 velocity;
    public GameObject Boss;
    public float timer, timerTot;
    public Camera camera;

    public float offsetCamera;

    public GameObject canvas;

    public CinematicBars cinematic;

    private void Awake()
    {
        camera = Camera.main;
        offsetCamera = 14;
    }

    private void FixedUpdate()
    {
        if(detected)
        {
            cinematic.Show(200, 0.8f);
            canvas.SetActive(false);
            camera.GetComponent<Camera_Focus>().enabled = false;
            var desactivate = GameObject.FindGameObjectsWithTag("player");
            for (int i = 0; i < desactivate.Length; i++)
            {
                //desactivate[i].GetComponent<Animator>().enabled = false;              
                //desactivate[i].GetComponent<Player_Movement>().enabled = false;
                desactivate[i].GetComponent<Player_Movement>().Stop_Moving();
            }
            camera.transform.position = Vector3.SmoothDamp(Camera.main.transform.position,new Vector3(Boss.transform.position.x, Boss.transform.position.y - offsetCamera,camera.transform.position.z), ref velocity, smoothTime);
            Boss.transform.position =  Vector2.Lerp(Boss.transform.position, new Vector2(Boss.transform.position.x, Boss.transform.position.y - 0.082f),0.5f);
            offsetCamera = Mathf.Lerp(offsetCamera, 2.5f, 0.007f);
            timer += Time.deltaTime;
            if(timer > timerTot)
            {
                for (int i = 0; i < desactivate.Length; i++)
                {
                    if(Boss.GetComponent<Collider2D>().GetType() == typeof(CapsuleCollider2D)) Boss.GetComponent<Collider2D>().enabled = true;
                }
                GetComponent<StartCombatBossGestion>().enabled = true;
                Boss.GetComponent<Animator>().enabled = true;
                Destroy(this);
            }
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "player" && !detected)
        {
            detected = true;
        }
    }
}
