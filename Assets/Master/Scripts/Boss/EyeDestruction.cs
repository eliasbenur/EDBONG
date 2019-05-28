using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeDestruction : MonoBehaviour
{
    //Detect all the players and decide for a target, which is the closer one
    List<GameObject> allPlayers = new List<GameObject>();

    //Variables we have to check to know if a monster can be cut, if so, then we trigger audioSource, animation
    public List<encer_trig> list_trig;
    public Rope_System_Elast rope_system;
    public AudioSource hit_lasser;
    public AudioSource audio_explosion;
    public GameObject blood_explo;
    bool dead;
    float timerCut;
    float timerCut_TOT;

    int num_trig = 0;
    float num_triggered;

    //Sign / gamefeel 

    //Camera Shake Effect
    // Transform of the GameObject you want to shake
    public Transform cameraTransform;
    public float shakeDuration;
    public float shakeMagnitude;
    public float dampingSpeed;
    public Vector3 initialPosition;

    public Boss boss;

    bool test;

    public enum MethodToKill
    {
        Cut = 1,
    }
    public MethodToKill method;


    private void Awake()
    {
        boss = GameObject.Find("Boss").GetComponent<Boss>();
        foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("player"))
        {
            allPlayers.Add(Obj);
        }

        cameraTransform = Camera.main.GetComponent<Transform>();
        //We find the Rope System, the target will be the center of the cain
        if (rope_system == null)
        {
            rope_system = GameObject.Find("Rope_System").GetComponent<Rope_System_Elast>();
        }
    }

    // Use this for initialization
    void Start()
    {
        dead = false;
        foreach (Transform child in transform)
        {
            if (child.name != "CleanCollision")
                list_trig.Add(child.GetComponent<encer_trig>());
        }

        //Method allows us to choose the type of monster we want to have
        switch (method)
        {
            case MethodToKill.Cut:
                num_triggered = 3;
                timerCut_TOT = 0.28f;
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
        initialPosition = Camera.main.transform.position;

        //Start surround check how many colliders are triggered by the rope, if we have for example 3 triggered on 8 then we turn on the timer of cut 
        //   ->    it's a light delay to have the feelings of real cutting

        Start_surround();

        if (method == MethodToKill.Cut)
        {
            if (num_trig >= num_triggered)
            {
                if (allPlayers[0].GetComponent<Player_Movement>().get_MovementX() != 0 || allPlayers[0].GetComponent<Player_Movement>().get_MovementY() != 0 /*&&  allPlayers[1].GetComponent<Player2_Movement>().moveX != 0 || allPlayers[1].GetComponent<Player2_Movement>().moveY != 0*/)
                {
                    timerCut += Time.deltaTime;
                    if (timerCut > timerCut_TOT)
                    {
                        allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
                        allPlayers[1].GetComponent<Player_Movement>().testVibrationHitRope = true;
                        GetComponent<CircleCollider2D>().enabled = false;
                        if (gameObject.name == "RightEye(Clone)")
                        {
                            StartCoroutine(Dead());
                            if(test)
                            {
                                for (int i=0;i < boss.all_Childrens.Count;i++)
                                {
                                    if (boss.all_Childrens[i].name == "RightEye")
                                    {
                                        boss.all_Childrens.RemoveAt(i);
                                    }
                                }
                            }
                        }
                        else if (gameObject.name == "LeftEye(Clone)")
                        {
                            StartCoroutine(Dead());
                            if (test)
                            {
                                for (int i = 0; i < boss.all_Childrens.Count; i++)
                                {
                                    if (boss.all_Childrens[i].name == "LeftEye")
                                    {
                                        boss.all_Childrens.RemoveAt(i);
                                    }
                                }
                            }
                        }
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
                if (allPlayers[0].GetComponent<Player_Movement>().get_MovementX() != 0 || allPlayers[0].GetComponent<Player_Movement>().get_MovementY() != 0 /*&&  allPlayers[1].GetComponent<Player2_Movement>().moveX != 0 || allPlayers[1].GetComponent<Player2_Movement>().moveY != 0*/)
                {
                    timerCut += Time.deltaTime;
                    if (timerCut > timerCut_TOT)
                    {
                        allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
                        allPlayers[1].GetComponent<Player_Movement>().testVibrationHitRope = true;
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

    float GetDistance(GameObject obj)
    {
        float distance = Vector2.Distance(obj.transform.position, transform.position);
        return distance;
    }

    //When an enemy collide with a player, he stop moving to avoid some shakings 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "player")
        {
            allPlayers[0].GetComponent<JoysticVibration_Manager>().alreadyVibrated = false;
            allPlayers[1].GetComponent<JoysticVibration_Manager>().alreadyVibrated = false;
        }
    }

    IEnumerator Dead()
    {
        if (!dead)
        {
            if (boss.all_Childrens.Count == 3)
                boss.GetComponent<Animator>().Play("BossHited_Phase1");
            else if (boss.all_Childrens.Count == 2)
            {
                boss.GetComponent<Animator>().Play("Boss_Hited_Phase2");
            }
            boss.checkBeforeNewPhase = false;
            boss.alreadyPlay = false;
            boss.timerCondition = 0;
            boss.timerCondition2 = 0;
            boss.x = 0;
            boss.y = 0;
            boss.angle = 90;
            boss.stop = false;
            boss.retour = false;
            boss.aller = true;
            //boss.canSpawn = true;
            dead = true;
            
            boss.cameraMoving = true;
            boss.returnCamera = false;
            boss.CameraTestOneTime = false;
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
            test = true;
            yield return new WaitForSeconds(1.1f);
            audio_explosion.Play();
            Instantiate(blood_explo, new Vector3(transform.position.x, transform.position.y, blood_explo.transform.position.z), blood_explo.transform.rotation);
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
            
        }
    }
}
