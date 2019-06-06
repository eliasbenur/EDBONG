using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class IA_Choice_CUT_SURROUND_DASH : MonoBehaviour
{
    #region PLAYERS
    //Detect all the players and decide for a target, which is the closer one
    //PLAYERS
    GameObject target;
    [Header("Distance Monster Settings")]
    public float detectionDistance_minimalBeforeLeave, distanceShoot;
    public float detectionDistance;
    public float projectileToFire;
    public float cooldown, cooldown_betweenNextProejctile;
    public float speedProjectile;
    public RippleEffect shockwave;
    Animator animator;
    #endregion

    #region AttackCutMonster
    //Time before the monster is enable to attack, time + animation
    [Header("Cac Monster Settings")]
    public float timer_BeforeAttack;
    public float cut_delay;
    float timer;
    bool atack_in_range;   
    bool p1_inRange;
    bool p2_inRange;
    #endregion

    #region AttackShootMonster   
    bool canShoot = true;
    IEnumerator coroutineFire; 
    #endregion

    #region CutMonster
    //Variables we have to check to know if a monster can be cut, if so, then we trigger audioSource, animation   
    float timerCut;
    float timerCut_TOT; 
    #endregion

    #region Surround Monster
    bool confirmed;
    GameObject ropeSystemGetChild;
    #endregion

    #region SPAWN / General Behavior
    private Blinking_Effect blink;  
    int num_trig = 0;
    float num_triggered;
    [HideInInspector] public List<encer_trig> list_trig;
    private Rope_System rope_system;
    [Header("General Attributs")]
    public GameObject blood_explo;
    //Old speed is used to get back the speed, after he was to the contact of the player
    public float enemySpeed;
    float oldSpeed;
    //Bool allow the monster to attack
    bool anim_atack;
    bool dead;
    private float idle_anim_time;
    private moneyDrop moneyDrop;
    #endregion

    #region Kamikaza
    float angle;
    [Header("Kamikaza Monster Settings")]
    public float projectileToSpawn;
    public float timer_BeforeExplosion;
    public float angleToADD;
    #endregion

    #region Camera
    //Camera Shake Effect
    // Transform of the GameObject you want to shake
    private Transform cameraTransform;
    private float shakeDuration;
    private float shakeMagnitude;
    private Vector3 initialPosition;
    private List<Player_Movement> players;
    #endregion

    public enum MethodToKill
    {
        Cut = 1,
        Surround = 2,
        Dash = 3,
    }
    public enum MethodAttack
    {
        Cac = 1,
        DistanceShoot = 2,
    }
    public enum MethodToDie
    {
        Normal = 1,
        Kamikaze =2,
    }

    [Header("Method Kill/Die/Attack Settings")]
    public MethodToKill methodToKill;
    public MethodAttack methodAttack;
    public MethodToDie methodToDie;

    private void Awake()
    {
        oldSpeed = enemySpeed;
        animator = GetComponent<Animator>();
        if (rope_system == null)
            rope_system = GameObject.Find("Rope_System").GetComponent<Rope_System>();
        blink = GetComponent<Blinking_Effect>();
        ropeSystemGetChild = rope_system.gameObject;
        cameraTransform = Camera.main.GetComponent<Transform>();
        players = Camera.main.GetComponent<GameManager>().players_Movement;
        moneyDrop = GetComponent<moneyDrop>();

        shockwave = Camera.main.GetComponent<RippleEffect>();
    }

    // Use this for initialization
    void Start()
    {
        if (methodToDie == MethodToDie.Kamikaze)
            speedProjectile = 400;
        dead = false;
        foreach (Transform child in transform)
        {
            if (child.name != "CleanCollision")
                list_trig.Add(child.GetComponent<encer_trig>());
        }
        //Method allows us to choose the type of monster we want to have
        switch (methodToKill)
        {
            case MethodToKill.Cut:
                num_triggered = 3;
                timerCut_TOT = cut_delay;
                break;
            case MethodToKill.Dash:
                num_triggered = 8;
                break;
            case MethodToKill.Surround:
                num_triggered = 8;
                timerCut_TOT = 0.7f;
                break;
            default:
                methodToKill = MethodToKill.Cut;
                num_triggered = 3;
                timerCut_TOT = 0.28f;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Noise();

        if (blink.spawn)
            blink.SpriteBlinkingEffect();
        else
        {
            if (target != null)
            {
                //If one player (who are not the actual target) is closer than the target, then the script change of target
                var maxDistance = float.MaxValue;
                for(int i = 0; i < players.Count; i++)
                { 
                    var whichOneCloser = GetDistance(players[i].gameObject);
                    if (whichOneCloser < maxDistance)
                    {
                        target = players[i].gameObject;
                        maxDistance = whichOneCloser;
                    }
                }
                //Condition to turn animations on
                if (methodAttack == MethodAttack.Cac)
                {
                    if (anim_atack && methodToDie != MethodToDie.Kamikaze)
                        AttackCac();
                    else
                        timer = 0;
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);

                    if (GetDistance(target) < detectionDistance)
                    {
                        Follow();
                        if (methodToDie != MethodToDie.Kamikaze)
                        {
                            if (animator != null)
                                animator.SetBool("running", true);
                        }
                    }
                    else
                    {
                        if (methodToDie != MethodToDie.Kamikaze)
                        {
                            if (animator != null)
                                animator.SetBool("running", false);
                        }
                    }
                }
                else if(methodAttack == MethodAttack.DistanceShoot)
                {
                    if (GetDistance(target) < distanceShoot)
                    {
                        if (canShoot)
                        {
                            if (GetDistance(target) > 4)
                            {
                                coroutineFire = FireCoroutine(cooldown);
                                StartCoroutine(coroutineFire);
                            }
                        }
                    }
                    //But if he's too close, then he leave in the opposite direction of players
                    if (GetDistance(target) < detectionDistance_minimalBeforeLeave)
                        Leave();
                    else
                        Follow();
                }         
            }

            //If the monster don't have target then we look for one
            if (target == null)
            {
                var maxDistance = float.MaxValue;
                for(int i=0; i < players.Count; i++)
                { 
                    var whichOneCloser = GetDistance(players[i].gameObject);
                    if (whichOneCloser < maxDistance)
                    {
                        target = players[i].gameObject;
                        maxDistance = whichOneCloser;
                    }
                }
            }

            Start_surround();
            if (methodToKill == MethodToKill.Surround)
            {                   
                switch (num_trig)
                {
                    case 4:
                        shakeDuration = 1;
                        shakeMagnitude = 0.04f;
                        CameraShake();
                        foreach (Transform child in ropeSystemGetChild.transform)
                        {                                
                            child.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 255);
                        }
                        break;

                    case 5:
                        shakeDuration = 1;
                        shakeMagnitude = 0.05f;
                        CameraShake();
                        foreach (Transform child in ropeSystemGetChild.transform)
                        {
                            child.GetComponent<SpriteRenderer>().color = new Color(255, 150, 0, 255);
                        }
                        break;

                    case 6:
                        shakeDuration = 1;
                        shakeMagnitude = 0.06f;
                        CameraShake();
                        foreach (Transform child in ropeSystemGetChild.transform)
                        {
                            child.GetComponent<SpriteRenderer>().color = new Color(255, 255, 0, 255);
                        }
                        break;

                    case 7:
                        shakeDuration = 1;
                        shakeMagnitude = 0.07f;
                        CameraShake();
                        foreach (Transform child in ropeSystemGetChild.transform)
                        {
                            child.GetComponent<SpriteRenderer>().color = new Color(150, 255, 0, 255);
                        }
                        break;

                    case 8:
                        shakeDuration = 0;
                        shakeMagnitude = 0;
                        foreach (Transform child in ropeSystemGetChild.transform)
                        {
                            child.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0, 255);
                        }
                        if (!confirmed)
                        {
                            shockwave.enabled = true;
                            confirmed = true;
                        }
                        break;

                    default:
                        shakeDuration = 0;
                        shakeMagnitude = 0;
                        foreach (Transform child in ropeSystemGetChild.transform)
                        {
                            child.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
                        }
                        confirmed = false;
                        break;
                }
            }

            if (num_trig >= num_triggered)
            {
                if (Players_dashing())
                {
                    if (methodToKill == MethodToKill.Cut && methodToDie != MethodToDie.Kamikaze || methodToKill == MethodToKill.Surround)
                    {
                        players[0].testVibrationHitRope = true;
                        players[1].testVibrationHitRope = true;
                        if (animator != null)
                            animator.SetBool("dead", true);
                        gameObject.GetComponent<Collider2D>().enabled = false;
                        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                        transform.localScale = new Vector3(1, 1, 1);
                        StartCoroutine(Dead());
                    }
                }

                if (players[0].get_MovementX() != 0 || players[1].get_MovementY() != 0)
                {
                    timerCut += Time.deltaTime;
                    if (methodToKill == MethodToKill.Surround)
                        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.6f, 1, 1), timerCut / timerCut_TOT);

                    if (timerCut > timerCut_TOT)
                    {
                        players[0].testVibrationHitRope = true;
                        players[1].testVibrationHitRope = true;

                        gameObject.GetComponent<Collider2D>().enabled = false;
                        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                        transform.localScale = new Vector3(1, 1, 1);

                        if (methodToDie == MethodToDie.Normal)
                        {
                            animator.SetBool("dead", true);
                            StartCoroutine(Dead());
                        }
                        else if (methodToDie == MethodToDie.Kamikaze)
                        {
                            blink.spriteBlinkingTotalDuration = timer_BeforeExplosion;
                            blink.spriteBlinkingMiniDuration = 0.1f;
                            blink.spawn = true;
                            StartCoroutine(Wait_EXPLOSIONN());
                        }
                                
                    }
                }
                else
                    timerCut = 0;
            }
            else
            {
                timerCut = 0;
                transform.localScale = new Vector3(1,1,1);
            }
        }
    }

    void Noise()
    {
        if (idle_anim_time > 0)
        {
            idle_anim_time -= Time.fixedDeltaTime;
        }
        if (idle_anim_time <= 0)
        {
            idle_anim_time = Random.Range(5.0f, 10.0f);
        }
    }

    void Leave()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Time.deltaTime * -enemySpeed);
    }

    public bool Players_dashing()
    {
        return (players[0].Dashing() || players[1].Dashing());
    }

    void Start_surround()
    {
        num_trig = 0;
        foreach (encer_trig trig in list_trig)
        {
            if (trig.Check_isTouching())
                num_trig++;
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
        if ((target.transform.position - transform.position).x > 0)
            transform.GetComponent<SpriteRenderer>().flipX = true;
        else
            transform.GetComponent<SpriteRenderer>().flipX = false;
    }

    void CameraShake()
    {
        initialPosition = Camera.main.transform.position;
        if (shakeDuration > 0)
        {
            cameraTransform.localPosition = initialPosition + UnityEngine.Random.insideUnitSphere * shakeMagnitude;
        }
        else
        {
            cameraTransform.localPosition = initialPosition;
        }
    }

    //When an enemy collide with a player, he stop moving to avoid some shakings 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "player")
        {
            enemySpeed = 0;
            if (methodAttack == MethodAttack.Cac)
            {
                //We allow him to attack, the bool attack will trigger a timer, before he's allowed to deal damage
                atack_in_range = true;
                anim_atack = true;
                if (other.gameObject.name == "PlayerOne")
                    p1_inRange = true;
                else
                    p2_inRange = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            enemySpeed = oldSpeed;
            if (methodAttack == MethodAttack.Cac)
            {
                atack_in_range = false;
                if (collision.gameObject.name == "PlayerOne")
                {
                    p1_inRange = false;
                }
                else
                {
                    p2_inRange = false;
                }
            }
        }
    }

    IEnumerator Dead()
    {
        if (!dead)
        {
            dead = true;
            shockwave.enabled = false;
            AkSoundEngine.PostEvent("plays_slicing", Camera.main.gameObject);
            AkSoundEngine.PostEvent("play_monster1death", Camera.main.gameObject);

            blink.SpriteBlinkingEffect();
            //Used to control the vibrations in both controllers
            players[0].testVibrationHitRope = true;
            players[1].testVibrationHitRope = true;

            GetComponent<SpriteRenderer>().color = Color.white;
            enemySpeed = 0;

            yield return new WaitForSeconds(0.2f);
            GetComponent<SpriteRenderer>().color = Color.white;

            yield return new WaitForSeconds(0.2f);
        
            Instantiate(blood_explo, new Vector3(transform.position.x, transform.position.y, blood_explo.transform.position.z), blood_explo.transform.rotation);
            if (moneyDrop != null)
                moneyDrop.enabled = true;
            
            yield return new WaitForSeconds(0.25f);
            
            Destroy(gameObject);      
        }
    }

    IEnumerator Wait_EXPLOSIONN()
    {
        if (!dead)
        {
            dead = true;            
            AkSoundEngine.PostEvent("plays_slicing", Camera.main.gameObject);
            AkSoundEngine.PostEvent("play_kamikazeboom_start", Camera.main.gameObject);
            GetComponent<CircleCollider2D>().enabled = false;
            detectionDistance = 0;
            enemySpeed = 0;
            GetComponent<Animator>().enabled = false;
            
            angle = 2 * Mathf.PI;

            yield return new WaitForSeconds(timer_BeforeExplosion);

            Instantiate(blood_explo, new Vector3(transform.position.x, transform.position.y, blood_explo.transform.position.z), blood_explo.transform.rotation);
            if (moneyDrop != null)
                moneyDrop.enabled = true;
            for (int i = 0; i < projectileToSpawn; i++)
            {
                angle += angleToADD;
                Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
                var instanceAddForce = Instantiate(Resources.Load("ShotDistance"), transform.position + direction, Quaternion.identity) as GameObject;
                var directionVect = instanceAddForce.transform.position - transform.position;
                instanceAddForce.GetComponent<Rigidbody2D>().AddForce(directionVect.normalized * speedProjectile);
            }
            yield return new WaitForSeconds(0.1f);

            AkSoundEngine.PostEvent("play_kamikazeboom_explosion", Camera.main.gameObject);

            Destroy(gameObject);
        }
    }

    IEnumerator FireCoroutine(float cooldown)
    {
        if (!dead)
        {
            for (int i = 0; i <= projectileToFire; i++)
            {
                //Projectiles are instantiate and will be targeting the closer player
                var instanceAddForce = Instantiate(Resources.Load("ShotDistance"), new Vector2(transform.position.x, transform.position.y), Quaternion.identity) as GameObject;
                instanceAddForce.GetComponent<Rigidbody2D>().AddForce((target.transform.position - transform.position).normalized * speedProjectile, ForceMode2D.Impulse);
                //We wait a short time, to let the previous element go more forward before spawing an other one 
                canShoot = false;
                yield return new WaitForSeconds(cooldown_betweenNextProejctile);
            }
            canShoot = false;
            yield return new WaitForSeconds(cooldown);
            canShoot = true;
        }
    }

    void AttackCac()
    {
        timer += Time.deltaTime;
        animator.SetBool("attack", true);
        if (timer > timer_BeforeAttack)
        {
            if (atack_in_range && !dead)
            {
                for(int i =0; i < players.Count; i++)
                { 
                    if (players[i].name == "PlayerOne" && p1_inRange && !p2_inRange)
                        players[i].GetComponent<God_Mode>().Hit_verification("PlayerOne", players[i].transform.position, "Monster Choise - " + methodToKill.ToString());
                    else if (players[i].name == "PlayerTwo" && p2_inRange && !p1_inRange)
                        players[i].GetComponent<God_Mode>().Hit_verification("PlayerTwo", players[i].transform.position, "Monster Choise - " + methodToKill.ToString());
                    else if (p1_inRange && p2_inRange)
                        players[i].GetComponent<God_Mode>().Hit_verification("TwoOfThem", players[i].transform.position, "Monster Choise - " + methodToKill.ToString());
                }
            }
            timer = 0;
            //atack_in_range = false;
            if (!atack_in_range)
            {
                anim_atack = false;
                animator.SetBool("attack", false);
            }
            AkSoundEngine.PostEvent("play_monster1attack", Camera.main.gameObject);
            if (!dead)
                enemySpeed = oldSpeed;
        }
    }
}
