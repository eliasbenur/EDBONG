using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Trigger : MonoBehaviour
{

    private BoxCollider2D triger_coll;
    private BoxCollider2D coll;
    private Vector2 autorun_position;

    private GameObject playerone, playertwo;

    private int NumPlayer_inside = 0;
    public bool autoruning;
    private Vector2 mov_p1, mov_p2;
    private int Num_points;

    public Sprite door_opened;
    public Sprite door_closed;

    public bool auto_run_1time;

    // Start is called before the first frame update
    void Start()
    {
        triger_coll = gameObject.GetComponents<BoxCollider2D>()[0];
        coll = gameObject.GetComponents<BoxCollider2D>()[1];
        autorun_position = gameObject.transform.GetChild(0).transform.position;
        Num_points = GameObject.Find("Rope_System").GetComponent<Rope_System>().NumPoints;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (autoruning)
        {
            Player_AutoRun();
        }
    }

    public void Stop_AutoRuning()
    {
        if (autoruning)
        {
            autoruning = false;
            autoruning = false;
            coll.enabled = true;
            playerone.GetComponent<Player_Movement>().can_move = true;
            playertwo.GetComponent<Player_Movement>().can_move = true;
            GetComponent<SpriteRenderer>().sprite = door_closed;
        }
    }

    void Player_AutoRun()
    {
        playerone.GetComponent<Player_Movement>().moveX = mov_p1.x * Time.fixedDeltaTime;
        playerone.GetComponent<Player_Movement>().moveY = mov_p1.y * Time.fixedDeltaTime;
        playertwo.GetComponent<Player_Movement>().moveX = mov_p2.x * Time.fixedDeltaTime;
        playertwo.GetComponent<Player_Movement>().moveY = mov_p2.y * Time.fixedDeltaTime;
    }

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
                playerone = GameObject.Find("PlayerOne");
                playertwo = GameObject.Find("PlayerTwo");
                mov_p1 = (autorun_position - (Vector2)playerone.transform.position).normalized;
                mov_p2 = (autorun_position - (Vector2)playertwo.transform.position).normalized;
                coll.enabled = false;
                playerone.GetComponent<Player_Movement>().can_move = false;
                playertwo.GetComponent<Player_Movement>().can_move = false;
                colli_gestion();
                GetComponent<SpriteRenderer>().sprite = door_opened;
                //Physics2D.IgnoreLayerCollision(9, 15);
                autoruning = true;
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

    void colli_gestion()
    {
        string point_p1 = "Point_0";
        string point_p2 = "Point_" + (Num_points - 1);
        List<Rope_Point> rps = GameObject.Find("Rope_System").GetComponent<Rope_System>().Points;
        for (int x = 0; x < Num_points; x++)
        {
            if (rps[x].gameObject.name != point_p1 && rps[x].gameObject.name != point_p2)
            {
                Physics2D.IgnoreCollision(rps[x].GetComponent<CircleCollider2D>(), coll);
            }

        }
    }

    /*private void OnCollisionStay2D(Collision2D collision)
    {
        string point_p1 = "Point_0";
        string point_p2 = "Point_" + (Num_points - 1);
        
        if (collision.gameObject.name != point_p1 && collision.gameObject.name != point_p2)
        {
            Physics2D.IgnoreCollision(collision.collider, coll);
        }
    }*/
}
