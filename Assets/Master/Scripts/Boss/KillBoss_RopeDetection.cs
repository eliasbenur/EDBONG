using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class KillBoss_RopeDetection : MonoBehaviour
{
    //Detect all the players and decide for a target, which is the closer one
    public List<GameObject> allPlayers = new List<GameObject>();
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

    public GameObject dieSmoke1, dieSmoke2;
    bool canSpawn = true;
    public float cooldown;

    public enum MethodToKill
    {
        Cut = 1,
        Surround = 2,
        Dash = 3,
    }
    public MethodToKill method;


    private void Awake()
    {
        foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("player"))
        {
            allPlayers.Add(Obj);
        }
        cameraTransform = Camera.main.GetComponent<Transform>();
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

        //gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
            StartCoroutine(Smoke_Dead());

        initialPosition = Camera.main.transform.position;

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
                    if(Player_dashing())
                    {
                        allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
                        allPlayers[1].GetComponent<Player_Movement>().testVibrationHitRope = true;

                    
                        GetComponentInParent<Collider2D>().enabled = false;
                        GetComponentInParent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                        StartCoroutine(Dead());
                    }
                    if (!confirmed && method == MethodToKill.Surround)
                    {
                        Instantiate(shockwave, transform.position, Quaternion.identity);
                        signConfirmed.Play();
                        confirmed = true;
                    }
                    if (allPlayers[0].GetComponent<Player_Movement>().movementX != 0 || allPlayers[0].GetComponent<Player_Movement>().movementY != 0 /*&&  allPlayers[1].GetComponent<Player2_Movement>().moveX != 0 || allPlayers[1].GetComponent<Player2_Movement>().moveY != 0*/)
                    {
                        timerCut += Time.deltaTime;
                        if (timerCut > timerCut_TOT)
                        {
                            allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
                            allPlayers[1].GetComponent<Player_Movement>().testVibrationHitRope = true;
                            
                            GetComponentInParent<Collider2D>().enabled = false;
                            GetComponentInParent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                            StartCoroutine(Dead());
                        }
                    }
                    else
                        timerCut = 0;
                }
                else
                {
                    timerCut = 0;
                }
                    
            }
            else
            {
                //If the ennemy is beatable just with a dash finish move
                if (num_trig >= num_triggered && Player_dashing())
                {
                    if (allPlayers[0].GetComponent<Player_Movement>().movementX != 0 || allPlayers[0].GetComponent<Player_Movement>().movementY != 0 /*&&  allPlayers[1].GetComponent<Player2_Movement>().moveX != 0 || allPlayers[1].GetComponent<Player2_Movement>().moveY != 0*/)
                    {
                        timerCut += Time.deltaTime;
                        if (timerCut > timerCut_TOT)
                        {
                            allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
                            allPlayers[1].GetComponent<Player_Movement>().testVibrationHitRope = true;
                            GetComponentInParent<CapsuleCollider2D>().enabled = false;
                            StartCoroutine(Dead());
                            dead = true;                            
                        }
                    }
                    else
                        timerCut = 0;
                }
                else
                    timerCut = 0;
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
        if (allPlayers[0].GetComponent<Player_Movement>().dash_tmp > (allPlayers[0].GetComponent<Player_Movement>().dash_delay - allPlayers[0].GetComponent<Player_Movement>().dash_time)
            || allPlayers[1].GetComponent<Player_Movement>().dash_tmp > (allPlayers[1].GetComponent<Player_Movement>().dash_delay - allPlayers[1].GetComponent<Player_Movement>().dash_time))
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

    IEnumerator Dead()
    {
        if (!dead)
        {
            dead = true;
            
            //Used to control the vibrations in both controllers
            allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
            allPlayers[1].GetComponent<Player_Movement>().testVibrationHitRope = true;

            GetComponentInParent<SpriteRenderer>().material = flash_sprite;
            GetComponentInParent<SpriteRenderer>().color = Color.white;


            if (!hit_lasser.isPlaying)
            {
                hit_lasser.Play();
            }
            SoundManager.PlaySound(SoundManager.Sound.PlayerSlicing, transform.position);

            yield return new WaitForSeconds(0.2f);
            GetComponentInParent<SpriteRenderer>().material = default_sprite;
            GetComponentInParent<SpriteRenderer>().color = Color.white;

            yield return new WaitForSeconds(0.2f);

            audio_explosion.Play();           
            Instantiate(blood_explo, new Vector3(transform.position.x, transform.position.y, blood_explo.transform.position.z), blood_explo.transform.rotation);

            yield return new WaitForSeconds(0.25f);
            GetComponentInParent<Animator>().Play("Die");
        }
    }

    IEnumerator Smoke_Dead()
    {     
        var position = Random.insideUnitSphere * 1.2f + transform.parent.gameObject.transform.position;
        var smokeToSpawn = Random.Range(1, 3);
        var size = Random.Range(0.5f, 1.75f);
        switch (smokeToSpawn)
        {
            case 1:
                var smoke = Instantiate(dieSmoke1, position, Quaternion.identity); 
                smoke.transform.localScale = new Vector2(size, size);
                break;

            case 2:
                smoke = Instantiate(dieSmoke2, position, Quaternion.identity);
                smoke.transform.localScale = new Vector2(size, size);
                break;
        }
        canSpawn = false;
        yield return new WaitForSeconds(cooldown);
        if (cooldown > 0.1)
        {
            cooldown *= 0.9f;
        }
        else
        {
            AnalyticsEvent.LevelComplete(SceneManager.GetActiveScene().name, new Dictionary<string, object>
            {
                { "HP", Camera.main.GetComponent<GameManager>().life },
                { "time_passed", Time.timeSinceLevelLoad },
                { "num_hits" , Camera.main.GetComponent<GameManager>().num_hits }
            });
            Camera.main.GetComponent<Menu_Manager>().EndScreen();
            Destroy(transform.parent.gameObject);          
        }
        yield return null;
    }
}
