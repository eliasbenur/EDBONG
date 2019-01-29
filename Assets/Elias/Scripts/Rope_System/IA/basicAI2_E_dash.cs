using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicAI2_E_dash : MonoBehaviour
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


    public int num_trig = 0;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (delay_spawn > 0)
        {
            delay_spawn -= Time.deltaTime;
        }
        else
        {
            if (target != null)
            {
                //If one player (who are not the actual target) is closer than the target, then the script change of target
                var maxDistance = float.MaxValue;
                foreach (var player in allPlayers)
                {
                    var whichOneCloser = GetDistance(player);
                    if (whichOneCloser < maxDistance)
                    {
                        target = player;
                        maxDistance = whichOneCloser;
                    }
                }
                if (GetDistance(target) < detectionDistance)
                {
                    Follow();
                    animator.SetBool("running", true);
                }
                else
                {
                    animator.SetBool("running", false);
                }
                if (anim_atack)
                {
                    timer += Time.deltaTime;
                    animator.SetBool("attack", true);
                    if (timer > timer_BeforeAttack)
                    {
                        if (attack)
                        {
                            Camera.main.GetComponent<GameManager>().Hit();
                        }
                        timer = 0;
                        anim_atack = false;
                        animator.SetBool("attack", false);
                        enemySpeed = oldSpeed;
                    }
                }
                else
                {
                    timer = 0;
                }

                //Look at the Target
                transform.LookAt(target.transform.position);
                transform.Rotate(new Vector2(0, 90));
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            }
        }

        if (/*transform.parent.GetComponent<Rooms>().stayedRoom && */target == null)
        {
            foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("player"))
            {
                allPlayers.Add(Obj);
            }
            var maxDistance = float.MaxValue;
            foreach (var player in allPlayers)
            {
                var whichOneCloser = GetDistance(player);
                if (whichOneCloser < maxDistance)
                {
                    target = player;
                    maxDistance = whichOneCloser;
                }
            }
        }

        Start_surround();
        if (num_trig >= 7 && Player_dashing())
        {
            if (allPlayers[0].GetComponent<Player_Movement>().moveX != 0 || allPlayers[0].GetComponent<Player_Movement>().moveY != 0 /*&&  allPlayers[1].GetComponent<Player2_Movement>().moveX != 0 || allPlayers[1].GetComponent<Player2_Movement>().moveY != 0*/)
            {
                animator.SetBool("dead", true);
                GetComponent<CircleCollider2D>().enabled = false;
                StartCoroutine(Dead());
            }
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

    void Start_surround()
    {
        num_trig = 0;
        foreach (encer_trig2 trig in list_trig)
        {
            if (trig.Check_isTouching())
            {
                num_trig++;
            }
        }


    }

    float GetDistance(GameObject obj)
    {
        float distance = Vector2.Distance(obj.transform.position, transform.position);
        return distance;
    }

    void Follow()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Time.deltaTime * enemySpeed);
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
            //if (collision.transform.parent.tag == "rope" && delay_spawn <= 0)
            //{
                /*animator.SetBool("dead", true);
                GetComponent<CircleCollider2D>().isTrigger = true;
                StartCoroutine(Dead());*/
            //}
        }

    }

    IEnumerator Dead()
    {
        if (!dead && delay_spawn <= 0)
        {
            dead = true;

            allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
            allPlayers[1].GetComponent<Player2_Movement>().testVibrationHitRope = true;

            if (!hit_lasser.isPlaying)
            {
                hit_lasser.Play();
            }
            enemySpeed = 0;
            yield return new WaitForSeconds(1.1f);
            audio_explision.Play();
            Instantiate(blood_explo, new Vector3(transform.position.x, transform.position.y, blood_explo.transform.position.z), blood_explo.transform.rotation);
            yield return new WaitForSeconds(0.5f);
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
}
