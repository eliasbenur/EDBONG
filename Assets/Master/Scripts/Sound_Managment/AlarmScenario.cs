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
    public bool alarm_activated = false;

    public List<GameObject> alarm_list;

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
                        gameObject.transform.GetChild(x).GetComponent<Door_Trigger>().Set_auto_run_1time(true);
                    }
                }
            }
        }
    }

    IEnumerator System_Failure()
    {
        SoundManager.PlaySound(SoundManager.Sound.VoiceAlarm);
        Active_alamrs();
        canSpeak = false;
        yield return new WaitForSeconds(cooldown);
        canSpeak = true;
    }

    public void Active_alamrs()
    {
        if (!alarm_activated)
        {
            alarm_activated = true;
            foreach(GameObject alarm_ in alarm_list)
            {
                alarm_.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    public void Desactive_alamrs()
    {
        foreach (GameObject alarm_ in alarm_list)
        {
            alarm_.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
