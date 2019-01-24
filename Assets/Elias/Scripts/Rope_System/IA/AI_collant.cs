using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_collant : MonoBehaviour
{
    public List<GameObject> allPlayers = new List<GameObject>();
    public float detectionDistance;
    public GameObject target;
    public float enemySpeed;
    private float oldSpeed;


    public Animator animator;

    public float delay_spawn;

    public float timer_BeforeAttack;
    public float timer;
    public bool attack;
    public bool anim_atack;

    public AudioSource hit_lasser;
    public AudioSource audio_explision;

    public GameObject blood_explo;

    bool dead;

    public List<encer_trig2> list_trig;
    public Rope_System rope_system;
    public bool rope_atachment;

    public float timerCut, timerCut_TOT;

    public int num_trig = 0;

    GameObject trou;
    public GameObject point_to_coll;

    private void Awake()
    {
        oldSpeed = enemySpeed;
    }


    // Use this for initialization
    void Start()
    {
        //Debug.Log(distancePreview);
        dead = false;
        foreach (Transform child in transform)
        {
            list_trig.Add(child.GetComponent<encer_trig2>());
        }

        if (rope_system == null)
        {
            rope_system = GameObject.Find("Rope_System").GetComponent<Rope_System>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (delay_spawn > 0)
        {
            delay_spawn -= Time.deltaTime;
        }
        else
        {
            if (target != null)
            {
                if (GetDistance(target) < detectionDistance)
                {
                    Follow();
                }
                if (anim_atack)
                {
                    timer += Time.deltaTime;
                    //animator.SetBool("attack", true);
                    if (timer > timer_BeforeAttack)
                    {
                        if (attack)
                        {
                            Camera.main.GetComponent<GameManager>().Hit();
                        }
                        timer = 0;
                        anim_atack = false;
                        //animator.SetBool("attack", false);
                        enemySpeed = oldSpeed;
                    }
                }
                else
                {
                    timer = 0;
                }

                //Look at the Target
                /*if (!dead)
                {
                    transform.LookAt(target.transform.position);
                    transform.Rotate(new Vector2(0, 90));
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
                }*/

            }
        }

        if (/*transform.parent.GetComponent<Rooms>().stayedRoom && */target == null)
        {
            foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("player"))
            {
                allPlayers.Add(Obj);
            }
            target = rope_system.Points[rope_system.NumPoints / 2].gameObject;
        }


    }


    float GetDistance(GameObject obj)
    {
        float distance = Vector2.Distance(obj.transform.position, transform.position);
        return distance;
    }

    void Follow()
    {

        if (point_to_coll != null)
        {
            transform.position = point_to_coll.transform.position;
        }
        else
        {
            //transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Time.deltaTime * enemySpeed);
            Vector3 Delta = target.transform.position - transform.position;
            gameObject.GetComponent<Rigidbody2D>().MovePosition(transform.position + Delta.normalized * Time.fixedDeltaTime * enemySpeed);
        }
    }


    //When an enemy collide with a player, he stop moving to avoid some shakings 
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "player")
        {
            enemySpeed = 0;
            attack = true;
            anim_atack = true;
            //animator.SetBool("attack", true);
            allPlayers[0].GetComponent<Player_Movement>().alreadyVibrated = false;
            allPlayers[1].GetComponent<Player2_Movement>().alreadyVibrated = false;

        }

        if (collision.transform.tag != "player" && collision.transform.tag != "monster")
        {
            //if (collision.transform.parent.transform.parent.tag == "rope")
            /*if (collision.transform.parent.tag == "rope" && delay_spawn <= 0)
            {
                animator.SetBool("dead", true);
                GetComponent<CircleCollider2D>().isTrigger = true;
                StartCoroutine(Dead());
            }*/
        }

    }

    IEnumerator Dead()
    {
        if (!dead && delay_spawn <= 0)
        {

            dead = true;

            allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
            allPlayers[1].GetComponent<Player2_Movement>().testVibrationHitRope = true;

            enemySpeed = 0;

            gameObject.GetComponent<CircleCollider2D>().isTrigger = true;

            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            attack = false;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "piege")
        {
            trou = collision.gameObject;
            StartCoroutine(Dead());
            //dead = true;
        }
    }

    public bool Player_dashing()
    {
        if (allPlayers[0].GetComponent<Player_Movement>().dash_v > (allPlayers[0].GetComponent<Player_Movement>().dash_delay - allPlayers[0].GetComponent<Player_Movement>().dash_time)
            || allPlayers[1].GetComponent<Player2_Movement>().dash_v > (allPlayers[1].GetComponent<Player2_Movement>().dash_delay - allPlayers[1].GetComponent<Player2_Movement>().dash_time))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
