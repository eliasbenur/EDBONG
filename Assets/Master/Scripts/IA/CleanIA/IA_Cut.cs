using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_Cut : MonoBehaviour
{
    //Detect all the players and decide for a target, which is the closer one
    public List<GameObject> allPlayers = new List<GameObject>();
    public GameObject target;
    public float detectionDistance;

    //Old speed is used to get back the speed, after he was to the contact of the player
    public float enemySpeed;
    float oldSpeed;

    //Time before the monster is enable to attack, time + animation
    float timer_BeforeAttack;
    float timer;
    bool attack;
    Animator animator;
    bool anim_atack;

    //Variables we have to check to know if a monster can be cut, if so, then we trigger audioSource, animation
    public List<encer_trig2> list_trig;
    float timerCut, timerCut_TOT;
    int num_trig = 0;
    public AudioSource hit_lasser;
    public AudioSource audio_explosion;
    public GameObject blood_explo;
    bool dead;
    
    private void Awake()
    {
        oldSpeed = enemySpeed;
        animator = GetComponent<Animator>();
        timer_BeforeAttack = 0.5f;
        timerCut_TOT = 0.28f;
    }

    void Start()
    {
        dead = false;
        foreach (Transform child in transform)
        {
            list_trig.Add(child.GetComponent<encer_trig2>());
        }
    }

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
                //when the monster is allow to attack, a timer is launch
                timer += Time.deltaTime;
                animator.SetBool("attack", true);
                if (timer > timer_BeforeAttack)
                {
                    if (attack)
                    {
                        Camera.main.GetComponent<GameManager>().Hit_p1();
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

        //Start surround check how many colliders are triggered by the rope, if we have 3 on 8 triggered then we turn on the timer of cut -> it's a light delay to have the feelings of real cutting
        Start_surround();

        if (num_trig >= 3)
        {
            if (allPlayers[0].GetComponent<Player_Movement>().moveX != 0 || allPlayers[0].GetComponent<Player_Movement>().moveY != 0)
            {
                timerCut += Time.deltaTime;
                if (timerCut > timerCut_TOT)
                {
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
