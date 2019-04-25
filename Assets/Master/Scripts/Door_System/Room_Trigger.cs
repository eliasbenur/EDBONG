using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class Room_Trigger : MonoBehaviour
{
    private bool players_inside;
    public int NumPlayer_inside = 0;
    public List<GameObject> enemy_list;
    private int num_doors = 0;

    public void Awake()
    {
        for (int x = 0; x < gameObject.transform.childCount; x++)
        {
            if (gameObject.transform.GetChild(0).tag == "door")
            {
                num_doors += 1;
            }
        }
    }


    private void Update()
    {
        if (gameObject.transform.childCount > 0 + num_doors)
        {
            if (gameObject.transform.GetChild(0).gameObject.transform.childCount == 0)
            {
                NextRound();
                Destroy(gameObject.transform.GetChild(0).gameObject);
            }else if (gameObject.transform.GetChild(0).tag == "door")
            {
                if (gameObject.transform.GetChild(0).GetChild(1).GetComponent<BoxCollider2D>().enabled)
                {
                    for (int x = 0; x < gameObject.transform.childCount; x++)
                    {
                        //gameObject.transform.GetChild(x).GetComponents<BoxCollider2D>()[1].enabled = false;
                        gameObject.transform.GetChild(x).GetChild(1).GetComponent<BoxCollider2D>().enabled = false;
                        //gameObject.transform.GetChild(x).GetComponents<BoxCollider2D>()[0].enabled = false;
                        //gameObject.transform.GetChild(x).GetComponent<SpriteRenderer>().sprite = gameObject.transform.GetChild(x).GetComponent<Door_Trigger>().door_opened;
                        gameObject.transform.GetChild(x).GetComponent<Animator>().SetBool("open", true);
                        Debug.Log("OPEN");
                    }

                    AnalyticsEvent.Custom("Room Completed", new Dictionary<string, object>
                    {
                        { "Scene", SceneManager.GetActiveScene().name },
                        { "Room" , transform.name }
                    });
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
        if (gameObject.transform.childCount > 1 + num_doors)
        {
            for (int x = 0; x < gameObject.transform.GetChild(1).gameObject.transform.childCount; x++)
            {
                gameObject.transform.GetChild(1).transform.GetChild(x).gameObject.SetActive(true);
            }
        }
        else{
            Debug.Log("NO Enemyes");
            Destroy(gameObject.transform.GetChild(0).gameObject);
            for (int x = 0; x < gameObject.transform.childCount; x++)
            {
                Debug.Log(gameObject.transform.GetChild(0).name);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            NumPlayer_inside++;

            if (NumPlayer_inside == 2)
            {
                FirstRound();
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            NumPlayer_inside--;
        }
    }
}
