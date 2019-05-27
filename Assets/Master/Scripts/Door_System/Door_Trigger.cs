using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class Door_Trigger : MonoBehaviour
{
    private BoxCollider2D triger_coll;
    private Vector2 autorun_position;
    public Player_Movement playerone, playertwo;

    private int NumPlayer_inside = 0;
    private bool autoruning;
    private Vector2 mov_p1, mov_p2;
    private int Num_points;

    public Sprite door_opened;
    public Sprite door_closed;

    private bool auto_run_1time;

    private GameManager playersList;
    private Animator animator;

    private void Awake()
    {
        playersList = Camera.main.GetComponent<GameManager>();
        animator = GetComponent<Animator>();
    } 

    void Start()
    {
        triger_coll = gameObject.GetComponents<BoxCollider2D>()[0];
        autorun_position = gameObject.transform.GetChild(0).transform.position;
        for(int i=0; i < playersList.players_Movement.Count; i++)
        {
            if (playersList.players_Movement[i].name == "PlayerOne")
                playerone = playersList.players_Movement[i];
            else
                playertwo = playersList.players_Movement[i];
        }
        Num_points = GameObject.Find("Rope_System").GetComponent<Rope_System>().NumPoints;
    }

    void Update()
    {
        if (autoruning)
            Player_AutoRun();
    }

    public void Stop_AutoRuning()
    {
        if (autoruning)
        {
            autoruning = false;
            gameObject.transform.GetChild(1).GetComponent<BoxCollider2D>().enabled = true;
            playerone.Allow_Moving();
            playertwo.Allow_Moving();
            animator.SetBool("open", false);
            SoundManager.PlaySound(SoundManager.Sound.DoorOpening_Closing, transform.position);
        }
    }

    public void Set_auto_run_1time(bool val)
    {
        auto_run_1time = val;
    }

    /* Auto movement to a point */
    void Player_AutoRun()
    {
        playerone.movementX = mov_p1.x * Time.fixedDeltaTime;
        playerone.movementY = mov_p1.y * Time.fixedDeltaTime;
        playertwo.movementX = mov_p2.x * Time.fixedDeltaTime;
        playertwo.movementY = mov_p2.y * Time.fixedDeltaTime;
    }

    /* When the 2 players are colliding the door, the door opens and the players start autoruning */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!auto_run_1time)
        {
            if (collision.tag == "player")
            {
                NumPlayer_inside++;
            }

            if (NumPlayer_inside == 2)
            {
                mov_p1 = (autorun_position - (Vector2)playerone.transform.position).normalized;
                mov_p2 = (autorun_position - (Vector2)playertwo.transform.position).normalized;
                StartCoroutine("Delay_Door");
                playerone.Stop_Moving();
                for (int i = 0; i < playersList.players_Movement.Count; i++)
                    playersList.players_Movement[i].can_move = false;
                GetComponent<SpriteRenderer>().sprite = door_opened;
                autoruning = true;

                AnalyticsEvent.Custom("Room Reached", new Dictionary<string, object>
                {
                    { "Scene", SceneManager.GetActiveScene().name },
                    { "Room" , transform.parent.name }
                });

                animator.SetBool("open", true);
                SoundManager.PlaySound(SoundManager.Sound.DoorOpening_Closing, transform.position);
                auto_run_1time = true;
            }
        }
    }

    IEnumerator Delay_Door()
    {
        yield return new WaitForSeconds(1);
        gameObject.transform.GetChild(1).gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            NumPlayer_inside--;
        }
    }
}
