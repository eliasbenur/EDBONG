using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class Room_Trigger : MonoBehaviour
{
    private bool players_inside;
    private int NumPlayer_inside = 0;
    private int num_doors = 0;
    private bool is_active = false;
    private Rope_System rope_system;
    

    public void Awake()
    {
        for (int x = 0; x < gameObject.transform.childCount; x++)
        {
            if (gameObject.transform.GetChild(0).tag == "door")
            {
                num_doors += 1;
            }
        }

        rope_system = GameObject.Find("Rope_System").GetComponent<Rope_System>();
    }

    private void Update() 
    {
        if (is_active)
        {
            if (gameObject.transform.childCount > 0 + num_doors)
            {
                // If the are no more enemies, the next round starts
                if (gameObject.transform.GetChild(0).gameObject.transform.childCount == 0)
                {
                    NextRound();
                    Destroy(gameObject.transform.GetChild(0).gameObject);
                }
                //If there are no more rounds, the doors opens
                else if (gameObject.transform.GetChild(0).tag == "door")
                {
                    is_active = false;

                    if (gameObject.transform.GetChild(0).GetChild(1).GetComponent<BoxCollider2D>().enabled)
                    {
                        for (int x = 0; x < gameObject.transform.childCount; x++)
                        {
                            if (gameObject.transform.GetChild(x).tag == "door")
                            {
                                gameObject.transform.GetChild(x).GetChild(1).GetComponent<BoxCollider2D>().enabled = false;
                                gameObject.transform.GetChild(x).GetComponent<Animator>().SetBool("open", true);
                            }
                            else
                            {
                                foreach (Transform child in gameObject.transform)
                                {
                                    Destroy(child.gameObject);
                                }
                                rope_system.reset_enemieColl();
                            }

                        }

                        AnalyticsEvent.Custom("Room Completed", new Dictionary<string, object>{
                            { "Scene", SceneManager.GetActiveScene().name },
                            { "Room" , transform.name }
                        });

                        //AkSoundEngine.PostEvent("play_dooropen", Camera.main.gameObject);
                    }
                }
            }
        }

    }

    private void FirstRound()
    {
        for (int x = 0; x < gameObject.transform.GetChild(0).gameObject.transform.childCount; x++)
        {
            gameObject.transform.GetChild(0).transform.GetChild(x).gameObject.SetActive(true);
        }
        
    }

    private void NextRound()
    {
        for (int x = 0; x < gameObject.transform.GetChild(1).gameObject.transform.childCount; x++)
        {
            gameObject.transform.GetChild(1).transform.GetChild(x).gameObject.SetActive(true);
        }
    }

    //When the 2 players enter in the room, the rounds starts
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            NumPlayer_inside++;

            if (NumPlayer_inside == 2)
            {
                is_active = true;
                FirstRound();
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            NumPlayer_inside--;
        }
    }
}
