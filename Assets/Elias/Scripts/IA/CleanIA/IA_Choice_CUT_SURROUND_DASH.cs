using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_Choice_CUT_SURROUND_DASH : MonoBehaviour
{
    //Detect all the players and decide for a target, which is the closer one
    List<GameObject> allPlayers = new List<GameObject>();
    GameObject target;
    public float detectionDistance;

    //Old speed is used to get back the speed, after he was to the contact of the player
    public float enemySpeed;
    float oldSpeed;

    //Time before the monster is enable to attack, time + animation
    float timer_BeforeAttack;
    float timer;
    bool attack;
    bool anim_atack;
    Animator animator;

    //Variables we have to check to know if a monster can be cut, if so, then we trigger audioSource, animation
    public List<encer_trig> list_trig;
    public Rope_System rope_system;
    public AudioSource hit_lasser;
    public AudioSource audio_explosion;
    public GameObject blood_explo;
    bool dead;
    float timerCut;
    float timerCut_TOT;

    int num_trig = 0;
    float num_triggered;


    public enum MethodToKill
    {
        Cut = 1,
        Surround = 2,
        Dash = 3,
    }
    public MethodToKill method;


    private void Awake()
    {
        oldSpeed = enemySpeed;
        animator = GetComponent<Animator>();
        timer_BeforeAttack = 0.5f;
        //We find the Rope System, the target will be the center of the cain
        if (rope_system == null)
        {
            rope_system = GameObject.Find("Rope_System").GetComponent<Rope_System>();
        }
    }

    // Use this for initialization
    void Start()
    {
        dead = false;
        foreach (Transform child in transform)
        {
            list_trig.Add(child.GetComponent<encer_trig>());
        }

        //Method allows us to choose the type of monster we want to have
        switch (method)
        {
            case MethodToKill.Cut:
                num_triggered = 3;
                timerCut_TOT = 0.28f;
                break;
            case MethodToKill.Dash:
                num_triggered = 8;
                break;
            case MethodToKill.Surround:
                num_triggered = 8;
                timerCut_TOT = 0.7f;
                break;
            //If we have forgotten to fill then by default it will be an ennemy to Cut
            default:
                method = MethodToKill.Cut;
                num_triggered = 3;
                timerCut_TOT = 0.28f;
                break;
        }
    }

    // Update is called once per frame
    void Update()
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
            //Condition to turn animations on
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
            //Since the LookAt method is a 3D method, we have to add a 90° rotation to be effective
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        }

        //If the monster don't have target then we look for one
        if (target == null)
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

        //Start surround check how many colliders are triggered by the rope, if we have for example 3 triggered on 8 then we turn on the timer of cut 
        //   ->    it's a light delay to have the feelings of real cutting

        Start_surround();

        if (method == MethodToKill.Cut || method == MethodToKill.Surround)
        {
            if (num_trig >= num_triggered)
            {
                if (allPlayers[0].GetComponent<Player_Movement>().moveX != 0 || allPlayers[0].GetComponent<Player_Movement>().moveY != 0 /*&&  allPlayers[1].GetComponent<Player2_Movement>().moveX != 0 || allPlayers[1].GetComponent<Player2_Movement>().moveY != 0*/)
                {
                    timerCut += Time.deltaTime;
                    if (timerCut > timerCut_TOT)
                    {
                        allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
                        allPlayers[1].GetComponent<Player_Movement>().testVibrationHitRope = true;
                        animator.SetBool("dead", true);
                        GetComponent<CircleCollider2D>().enabled = false;
                        StartCoroutine(Dead());
                    }
                }
                else
                    timerCut = 0;
            }
            else
                timerCut = 0;
        }
        else
        {
            //If the ennemy is beatable just with a dash finish move
            if (num_trig >= num_triggered && Player_dashing())
            {
                if (allPlayers[0].GetComponent<Player_Movement>().moveX != 0 || allPlayers[0].GetComponent<Player_Movement>().moveY != 0 /*&&  allPlayers[1].GetComponent<Player2_Movement>().moveX != 0 || allPlayers[1].GetComponent<Player2_Movement>().moveY != 0*/)
                {
                    timerCut += Time.deltaTime;
                    if (timerCut > timerCut_TOT)
                    {
                        allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
                        allPlayers[1].GetComponent<Player_Movement>().testVibrationHitRope = true;
                        animator.SetBool("dead", true);
                        GetComponent<CircleCollider2D>().enabled = false;
                        StartCoroutine(Dead());
                    }
                }
                else
                    timerCut = 0;
            }
            else
                timerCut = 0;
        }
    }

    public bool Player_dashing()
    {
        if (allPlayers[0].GetComponent<Player_Movement>().dash_v > (allPlayers[0].GetComponent<Player_Movement>().dash_delay - allPlayers[0].GetComponent<Player_Movement>().dash_time)
            || allPlayers[1].GetComponent<Player_Movement>().dash_v > (allPlayers[1].GetComponent<Player_Movement>().dash_delay - allPlayers[1].GetComponent<Player_Movement>().dash_time))
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
        foreach (encer_trig trig in list_trig)
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
            //We allow him to attack, the bool attack will trigger a timer, before he's allowed to deal damage
            attack = true;
            anim_atack = true;
            allPlayers[0].GetComponent<Player_Movement>().alreadyVibrated = false;
            allPlayers[1].GetComponent<Player_Movement>().alreadyVibrated = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            attack = false;
        }
    }

    IEnumerator Dead()
    {
        if (!dead)
        {
            dead = true;
            //Used to control the vibrations in both controllers
            allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
            allPlayers[1].GetComponent<Player_Movement>().testVibrationHitRope = true;
            if (audio_explosion == null || hit_lasser == null)
            {
                Instantiate(blood_explo, new Vector3(transform.position.x, transform.position.y, blood_explo.transform.position.z), blood_explo.transform.rotation);
                yield return new WaitForSeconds(0.5f);
                Destroy(gameObject);
            }
            if (!hit_lasser.isPlaying)
            {
                hit_lasser.Play();
            }
            enemySpeed = 0;
            yield return new WaitForSeconds(1.1f);
            audio_explosion.Play();           
            Instantiate(blood_explo, new Vector3(transform.position.x, transform.position.y, blood_explo.transform.position.z), blood_explo.transform.rotation);
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }
    }
}
