using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmScenario : MonoBehaviour
{
    private int NumPlayer_inside = 0;
    public AudioClip Alarm;
    bool audioReady;
    AudioSource audio;
    bool canSpeak;
    public float cooldown;
    IEnumerator audioAlarm;

    public void Awake()
    {
        audio = Camera.main.GetComponent<AudioSource>();
    }
    private void Update()
    {
        if(audioReady)
        {
            audio.volume -= Time.deltaTime * 0.15f;
            if (audio.volume == 0)
            {
                audio.clip = Alarm;
                audio.Play();
                audio.volume = 0.7f;
                audioAlarm = System_Failure();
                StartCoroutine(audioAlarm);
                audioReady = false;
            }
        }

        if (canSpeak)
        {
            audioAlarm = System_Failure();
            StartCoroutine(audioAlarm);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            NumPlayer_inside++;

            if (NumPlayer_inside == 2)
            {
                audioReady = true;
                for (int x = 0; x < gameObject.transform.childCount; x++)
                {
                    if (gameObject.transform.GetChild(x).tag == "door")
                    {
                        gameObject.transform.GetChild(x).GetComponent<Door_Trigger>().auto_run_1time = true;
                    }
                }
            }
        }
    }

    IEnumerator System_Failure()
    {
        SoundManager.PlaySound(SoundManager.Sound.VoiceAlarm);
        canSpeak = false;
        yield return new WaitForSeconds(cooldown);
        canSpeak = true;
    }
}
