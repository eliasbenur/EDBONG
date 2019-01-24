using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Kamikaza : MonoBehaviour
{
    private List<GameObject> allPlayers = new List<GameObject>();
    private GameObject target;

    bool attack;
    //Tweekable value
    public float detectionDistance;
    public float enemySpeed, oldSpeed;  
    public float timer, timer_BeforeAttack;
    public float timer_BeforeExplosion;
    IEnumerator Kamikaza;
    //Tweekable value for explosion on death
    private float angle;
    public float projectileToSpawn;
    public float angleToADD;
    public float speedProjectile;
    public float cooldown, cooldownToWait;
    bool dieQuick, canShoot;

    //Blincking Effect
    private float spriteBlinkingTimer;
    public float spriteBlinkingMiniDuration;
    public float spriteBlinkingTotalTimer;
    public float spriteBlinkingTotalDuration;
    public bool startBlinking = false;

    bool dead;

    public List<encer_trig2> list_trig;
    public Rope_System rope_system;
    public bool rope_atachment;

    public float timerCut, timerCut_TOT;

    public int num_trig = 0;
    public float delay_spawn;

    public AudioSource hit_lasser;
    public AudioSource audio_explision;

    public GameObject blood_explo;

    private void Awake()
    {
        oldSpeed = enemySpeed;
        AudioSource[] audios = gameObject.GetComponents<AudioSource>();
        hit_lasser = audios[0];
        audio_explision = audios[1];
    }

    private void Start()
    {
        foreach (Transform child in transform)
        {
            list_trig.Add(child.GetComponent<encer_trig2>());
        }
    }
    void Update()
    {
        if (startBlinking)
            SpriteBlinkingEffect();

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
            }
            if (attack)
            {
                timer += Time.deltaTime;
                if (timer > timer_BeforeAttack)
                {
                    Camera.main.GetComponent<GameManager>().Hit();
                    timer = 0;
                }
            }
            else
            {
                timer = 0;
            }
        }
        
        Start_surround();
        if (num_trig >= 8)
        {
            if (allPlayers[0].GetComponent<Player_Movement>().moveX != 0 || allPlayers[0].GetComponent<Player_Movement>().moveY != 0 /*&&  allPlayers[1].GetComponent<Player2_Movement>().moveX != 0 || allPlayers[1].GetComponent<Player2_Movement>().moveY != 0*/)
            {
                timerCut += Time.deltaTime;
                if (timerCut > timerCut_TOT)
                {
                    allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
                    allPlayers[1].GetComponent<Player2_Movement>().testVibrationHitRope = true;
                    GetComponent<CircleCollider2D>().enabled = false;
                    StartCoroutine(Wait_EXPLOSIONN());

                    /*for (int i = 0; i < transform.parent.GetComponent<Rooms>().currentEnnemies.Count; i++)
                    {
                        if (this.gameObject.transform == transform.parent.GetComponent<Rooms>().currentEnnemies[i])
                            transform.parent.GetComponent<Rooms>().currentEnnemies.RemoveAt(i);
                    }
                    var coinToDropRand = Random.Range(0, 2);
                    var coinCount = 0;
                    if (coinCount < coinToDropRand)
                    {
                        //Instantiate(coinToDrop, transform.position, Quaternion.identity);
                        Instantiate(Resources.Load("CoinAnim"), transform.position, Quaternion.identity);
                        coinCount++;
                    }*/
                }
            }
            else
                timerCut = 0;
        }
        else
            timerCut = 0;

        //Look at the Target
        if (target != null)
        {
            transform.LookAt(target.transform.position);
            transform.Rotate(new Vector2(0, 90));
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
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

    IEnumerator Wait_EXPLOSIONN()
    {
        if (!dead && delay_spawn <= 0)
        {
            dead = true;

            GetComponent<CircleCollider2D>().enabled = false;
            detectionDistance = 0;
            enemySpeed = 0;
            startBlinking = true;
            angle = 2 * Mathf.PI;

            yield return new WaitForSeconds(timer_BeforeExplosion);
            if (!hit_lasser.isPlaying)
            {
                hit_lasser.Play();
            }
            audio_explision.Play();
            Instantiate(blood_explo, new Vector3(transform.position.x, transform.position.y, blood_explo.transform.position.z), blood_explo.transform.rotation);
            for (int i = 0; i < projectileToSpawn; i++)
            {
                angle += angleToADD;
                Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
                var instanceAddForce = Instantiate(Resources.Load("ShotDistance"), transform.position + direction, Quaternion.identity) as GameObject;
                var directionVect = instanceAddForce.transform.position - transform.position;
                instanceAddForce.GetComponent<Rigidbody2D>().AddForce(directionVect.normalized * speedProjectile);
            }
            yield return new WaitForSeconds(0.8f);
            Destroy(gameObject);
        }       
    }

    float GetDistance(GameObject obj)
    {
        float distance = Vector2.Distance(obj.transform.position, transform.position);
        return distance;
    }

    void Follow()
    {
        //transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Time.deltaTime * enemySpeed);
        Vector3 Delta = transform.position - target.transform.position;
        gameObject.GetComponent<Rigidbody2D>().MovePosition(transform.position + Delta.normalized * enemySpeed);
    }

    private void SpriteBlinkingEffect()
    {
        spriteBlinkingTotalTimer += Time.deltaTime;
        if (spriteBlinkingTotalTimer >= spriteBlinkingTotalDuration)
        {
            startBlinking = false;
            spriteBlinkingTotalTimer = 0.0f;
            this.gameObject.GetComponent<SpriteRenderer>().enabled = true;   // according to your sprite
            return;
        }

        spriteBlinkingTimer += Time.deltaTime;
        if (spriteBlinkingTimer >= spriteBlinkingMiniDuration)
        {
            spriteBlinkingTimer = 0.0f;
            if (this.gameObject.GetComponent<SpriteRenderer>().enabled == true)
            {
                this.gameObject.GetComponent<SpriteRenderer>().enabled = false;  //make changes
            }
            else
            {
                this.gameObject.GetComponent<SpriteRenderer>().enabled = true;   //make changes
            }
        }
    }

    #region Fonction void ON

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            enemySpeed = 0;
            attack = true;
        }


        /*if (collision.gameObject.tag == "rope")
        {
            Kamikaza = Wait_EXPLOSIONN(timer_BeforeExplosion);
            StartCoroutine(Kamikaza);
        }*/
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            enemySpeed = oldSpeed;
            attack = false;
        }
    }

    #endregion

}
