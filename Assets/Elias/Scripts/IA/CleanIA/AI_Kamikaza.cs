using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Kamikaza : MonoBehaviour
{
    //Detect all the players, we'll need it for the vibration control, or even to not trigger behavior if players are not here
    private List<GameObject> allPlayers = new List<GameObject>();
    private GameObject target;

    //Tweekable value
    public float detectionDistance;
    public float enemySpeed, oldSpeed;  
    public float timer_BeforeExplosion;

    //Tweekable value for explosion on death
    float angle;
    public float projectileToSpawn;
    public float angleToADD;
    public float speedProjectile;
    public float cooldown, cooldownToWait;
    public AudioSource hit_lasser;
    public AudioSource audio_explision;
    public GameObject blood_explo;

    //Blincking Effect
    float spriteBlinkingTimer;
    public float spriteBlinkingMiniDuration;
    public float spriteBlinkingTotalTimer;
    public float spriteBlinkingTotalDuration;
    public bool startBlinking = false;

    bool dead;
    //Rope system will be used to found a target for the monster 
    public Rope_System rope_system;
    public List<encer_trig> list_trig;
    public bool rope_atachment;
    public float timerCut, timerCut_TOT;
    public int num_trig = 0;
    

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
            list_trig.Add(child.GetComponent<encer_trig>());
        }
    }
    void Update()
    {
        //If the monster is defeated then he blink
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
        }
        
        Start_surround();
        //If players are surrounding him and are moving then he exploded
        if (num_trig >= 8)
        {
            if (allPlayers[0].GetComponent<Player_Movement>().moveX != 0 || allPlayers[0].GetComponent<Player_Movement>().moveY != 0)
            {
                timerCut += Time.deltaTime;
                if (timerCut > timerCut_TOT)
                {
                    allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
                    allPlayers[1].GetComponent<Player_Movement>().testVibrationHitRope = true;
                    GetComponent<CircleCollider2D>().enabled = false;
                    StartCoroutine(Wait_EXPLOSIONN());
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
            //Look at the Target
            transform.LookAt(target.transform.position);
            transform.Rotate(new Vector2(0, 90));
            //Since the LookAt method is a 3D method, we have to add a 90° rotation to be effective
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
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

    IEnumerator Wait_EXPLOSIONN()
    {
        if (!dead)
        {
            dead = true;
            //Explosion's projectiles are displayed all around the monster
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
        Vector3 Delta = target.transform.position - transform.position;
        gameObject.GetComponent<Rigidbody2D>().MovePosition(transform.position + Delta.normalized * enemySpeed * Time.deltaTime);
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
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            enemySpeed = oldSpeed;
        }
    }
    #endregion
}
