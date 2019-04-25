using System.Collections;
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
    public AudioClip bossMusic;
    AudioSource audio;
    bool audioReady, lerpAudioBoss, alreadyPlaying;
    public GameObject stopAlarm;
    public float MaxVolumeBoss;
    public GameObject miniMap;

    private void Awake()
    {
        audio = Camera.main.GetComponent<AudioSource>();
        camera = Camera.main;
        offsetCamera = 14;
    }

    private void FixedUpdate()
    {
        if(audioReady)
        {
            audio.volume -= Time.deltaTime * 0.65f;
            stopAlarm.GetComponent<AlarmScenario>().StopAllCoroutines();
            if (audio.volume == 0)
            {
                lerpAudioBoss = true;
                audioReady = false;
            }
        }

        if(lerpAudioBoss)
        {
            audio.clip = bossMusic;
            if(!alreadyPlaying)
            {
                audio.Play();
                alreadyPlaying = true;
            }
            
            audio.volume += Time.deltaTime * 0.15f;
            if (audio.volume >= MaxVolumeBoss)
                lerpAudioBoss = false;
        }

        if(detected)
        {
            miniMap.SetActive(false);
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
            audioReady = true;
            GetComponent<Collider2D>().enabled = false;        
        }
    }
}
