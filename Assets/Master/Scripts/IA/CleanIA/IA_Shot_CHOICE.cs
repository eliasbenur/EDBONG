using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_Shot_CHOICE : MonoBehaviour
{
    //Detect all the players and decide for a target, which is the closer one
    List<GameObject> allPlayers = new List<GameObject>();
    GameObject target;

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

    public Material default_sprite;
    public Material flash_sprite;



    //Camera Shake Effect
    // Transform of the GameObject you want to shake
    public Transform cameraTransform;
    public float shakeDuration;
    public float shakeMagnitude;
    public float dampingSpeed;
    public Vector3 initialPosition;

    public AudioSource signConfirmed;
    public GameObject shockwave;
    bool confirmed;

    public float cut_delay;

    //SPAWN 
    public float spawn_delay;
    float delay_flash;

    public float projectileToFire;
    bool canShoot = true;
    public float detectionDistance, detectionDistance_minimalBeforeLeave, distanceShoot;
    public float cooldown, cooldown_betweenNextProejctile;
    IEnumerator coroutineFire;
    public float speedProjectile;


    public enum MethodToKill
    {
        Cut = 1,
        Surround = 2,
        Dash = 3,
    }
    public MethodToKill method;


    private void Awake()
    {
        cameraTransform = Camera.main.GetComponent<Transform>();
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
            if (child.name != "CleanCollision")
            {
                list_trig.Add(child.GetComponent<encer_trig>());
            }
        }

        //Method allows us to choose the type of monster we want to have
        switch (method)
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
            //If we have forgotten to fill then by default it will be an ennemy to Cut
            default:
                method = MethodToKill.Cut;
                num_triggered = 3;
                timerCut_TOT = 0.28f;
                break;
        }

        spawn_delay = 1;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (spawn_delay >= 0)
        {
            spawn_delay -= Time.deltaTime;
            delay_flash -= Time.deltaTime;
            if (delay_flash <= 0.2)
            {
                if (delay_flash <= 0)
                {
                    delay_flash = 0.4f;
                }
                GetComponent<SpriteRenderer>().material = default_sprite;
            }
            else
            {
                GetComponent<SpriteRenderer>().material = flash_sprite;
            }
            if (spawn_delay <= 0)
            {
                GetComponent<SpriteRenderer>().material = default_sprite;
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
        }
        else
        {
            initialPosition = Camera.main.transform.position;
            if (target != null)
            {
                //If one player (who are not the actual target) is closer than the target, then the script change his target
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
                //If he's close enough then he can shoot
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
                {
                    Leave();
                }
                else
                {
                    Follow();
                }
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
                if (method == MethodToKill.Surround)
                {
                    var ropeSystemGetChild = GameObject.Find("Rope_System");
                    switch (num_trig)
                    {
                        case 4:
                            shakeDuration = 1;
                            shakeMagnitude = 0.04f;
                            dampingSpeed = 0.04f;
                            foreach (Transform child in ropeSystemGetChild.transform)
                            {
                                //child.GetComponent<Prime31.SpriteLightColorCycler>().enabled = false;
                                child.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 255);

                            }
                            break;

                        case 5:
                            shakeDuration = 1;
                            shakeMagnitude = 0.05f;
                            dampingSpeed = 0.05f;
                            CameraShake();
                            foreach (Transform child in ropeSystemGetChild.transform)
                            {
                                //child.GetComponent<Prime31.SpriteLightColorCycler>().enabled = false;
                                child.GetComponent<SpriteRenderer>().color = new Color(255, 150, 0, 255);
                            }
                            break;

                        case 6:
                            shakeDuration = 1;
                            shakeMagnitude = 0.06f;
                            dampingSpeed = 0.06f;
                            CameraShake();
                            foreach (Transform child in ropeSystemGetChild.transform)
                            {
                                //child.GetComponent<Prime31.SpriteLightColorCycler>().enabled = false;
                                child.GetComponent<SpriteRenderer>().color = new Color(255, 255, 0, 255);
                            }
                            break;

                        case 7:
                            shakeDuration = 1;
                            shakeMagnitude = 0.07f;
                            dampingSpeed = 0.07f;
                            CameraShake();
                            foreach (Transform child in ropeSystemGetChild.transform)
                            {
                                //child.GetComponent<Prime31.SpriteLightColorCycler>().enabled = false;
                                child.GetComponent<SpriteRenderer>().color = new Color(150, 255, 0, 255);
                            }
                            break;

                        case 8:
                            shakeDuration = 0;
                            shakeMagnitude = 0;
                            dampingSpeed = 0;
                            foreach (Transform child in ropeSystemGetChild.transform)
                            {
                                //child.GetComponent<Prime31.SpriteLightColorCycler>().enabled = false;
                                child.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0, 255);
                            }
                            break;

                        default:
                            shakeDuration = 0;
                            shakeMagnitude = 0;
                            dampingSpeed = 0;
                            foreach (Transform child in ropeSystemGetChild.transform)
                            {
                                //child.GetComponent<Prime31.SpriteLightColorCycler>().enabled = true;
                                child.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
                            }
                            confirmed = false;
                            break;
                    }
                }

                if (num_trig >= num_triggered)
                {
                    if (!confirmed && method == MethodToKill.Surround)
                    {
                        Instantiate(shockwave, transform.position, Quaternion.identity);
                        signConfirmed.Play();
                        confirmed = true;
                    }
                    if (allPlayers[0].GetComponent<Player_Movement>().moveX != 0 || allPlayers[0].GetComponent<Player_Movement>().moveY != 0 /*&&  allPlayers[1].GetComponent<Player2_Movement>().moveX != 0 || allPlayers[1].GetComponent<Player2_Movement>().moveY != 0*/)
                    {
                        timerCut += Time.deltaTime;
                        if (method == MethodToKill.Surround)
                        {
                            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.6f, 1, 1), timerCut / timerCut_TOT);
                        }
                        if (timerCut > timerCut_TOT)
                        {
                            allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
                            allPlayers[1].GetComponent<Player_Movement>().testVibrationHitRope = true;
                            animator.SetBool("dead", true);
                            gameObject.GetComponent<Collider2D>().enabled = false;
                            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                            transform.localScale = new Vector3(1, 1, 1);
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
                            GetComponent<Collider2D>().enabled = false;
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

    }

    void CameraShake()
    {
        if (shakeDuration > 0)
        {
            cameraTransform.localPosition = initialPosition + UnityEngine.Random.insideUnitSphere * shakeMagnitude;
        }
        else
        {
            cameraTransform.localPosition = initialPosition;
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
                //canShoot = true;
            }
            canShoot = false;
            yield return new WaitForSeconds(cooldown);
            canShoot = true;
        }
    }

    //If he leave, then he go in the opposite direction of the player
    void Leave()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Time.deltaTime * -enemySpeed);
    }

    void Follow()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Time.deltaTime * enemySpeed);

    }

    //When an enemy collide with a player, he stop moving to avoid some shakings 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "player")
        {
            enemySpeed = 0;
            //We allow him to attack, the bool attack will trigger a timer, before he's allowed to deal damage
            attack = true;
            anim_atack = true;
            //allPlayers[0].GetComponent<Player_Movement>().alreadyVibrated = false;
            //allPlayers[1].GetComponent<Player_Movement>().alreadyVibrated = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            attack = false;
            enemySpeed = oldSpeed;
        }
    }

    IEnumerator Dead()
    {
        if (!dead)
        {
            dead = true;
            enemySpeed = 0;
            //Used to control the vibrations in both controllers
            allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
            allPlayers[1].GetComponent<Player_Movement>().testVibrationHitRope = true;
            GetComponent<SpriteRenderer>().material = flash_sprite;
            GetComponent<SpriteRenderer>().color = Color.white;
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
            yield return new WaitForSeconds(0.2f);
            GetComponent<SpriteRenderer>().material = default_sprite;
            GetComponent<SpriteRenderer>().color = Color.white;
            yield return new WaitForSeconds(1f);
            audio_explosion.Play();
            Instantiate(blood_explo, new Vector3(transform.position.x, transform.position.y, blood_explo.transform.position.z), blood_explo.transform.rotation);
            Destroy(gameObject);
        }
    }
}
