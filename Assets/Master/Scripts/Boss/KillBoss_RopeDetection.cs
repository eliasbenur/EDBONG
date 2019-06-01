using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class KillBoss_RopeDetection : MonoBehaviour
{
    #region Properties
    //Detect all the players and decide for a target, which is the closer one
    [HideInInspector] public List<GameObject> allPlayers = new List<GameObject>();
    //Variables we have to check to know if a monster can be cut, if so, then we trigger audioSource, animation
    [HideInInspector] public List<encer_trig> list_trig;
    [HideInInspector] public Rope_System rope_system;
    public GameObject blood_explo;
    private bool dead;
    private float timerCut;
    private float timerCut_TOT;

    private int num_trig = 0;
    private float num_triggered;

    public Material default_sprite;
    public Material flash_sprite;

    //Camera Shake Effect
    private Transform cameraTransform;
    private float shakeDuration;
    private float shakeMagnitude;
    private Vector3 initialPosition;

    public GameObject shockwave;
    private bool confirmed;

    public GameObject dieSmoke1, dieSmoke2;
    public float cooldown;
    private SpriteRenderer sprite;
    private List<Player_Movement> players;

    //Mashing 
    public GameObject mashingCanvas;
    public MashingController mashing;
    #endregion

    public enum MethodToKill
    {
        Surround = 1,
    }
    public MethodToKill method;

    private void Awake()
    {
        players = Camera.main.GetComponent<GameManager>().players_Movement;

        foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("player"))        
            allPlayers.Add(Obj);
        cameraTransform = Camera.main.GetComponent<Transform>();
        //We find the Rope System, the target will be the center of the chain
        if (rope_system == null)
            rope_system = GameObject.Find("Rope_System").GetComponent<Rope_System>();
        sprite = GetComponentInParent<SpriteRenderer>();
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
        switch (method)
        {
            case MethodToKill.Surround:
                num_triggered = 8;
                timerCut_TOT = 0.7f;
                break;
            default:
                method = MethodToKill.Surround;
                num_triggered = 8;
                timerCut_TOT = 0.7f;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mashing.confirmed)
        {
            players[0].testVibrationHitRope = true;
            players[1].testVibrationHitRope = true;

            GetComponentInParent<Collider2D>().enabled = false;
            GetComponentInParent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            StartCoroutine(Dead());
            mashingCanvas.SetActive(false);
        }


        if (dead)
            StartCoroutine(Smoke_Dead());

        initialPosition = Camera.main.transform.position;

        Start_surround();

        if (method == MethodToKill.Surround)
        {
            var ropeSystemGetChild = GameObject.Find("Rope_System");
            switch (num_trig)
            {
                case 4:
                    shakeDuration = 1;
                    shakeMagnitude = 0.04f;
                    foreach (Transform child in ropeSystemGetChild.transform)
                        child.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 255);
                    break;

                case 5:
                    shakeDuration = 1;
                    shakeMagnitude = 0.05f;
                    CameraShake();
                    foreach (Transform child in ropeSystemGetChild.transform)
                        child.GetComponent<SpriteRenderer>().color = new Color(255, 150, 0, 255);
                    break;

                case 6:
                    shakeDuration = 1;
                    shakeMagnitude = 0.06f;
                    CameraShake();
                    foreach (Transform child in ropeSystemGetChild.transform)
                        child.GetComponent<SpriteRenderer>().color = new Color(255, 255, 0, 255);
                    break;

                case 7:
                    shakeDuration = 1;
                    shakeMagnitude = 0.07f;
                    CameraShake();
                    foreach (Transform child in ropeSystemGetChild.transform)                            
                        child.GetComponent<SpriteRenderer>().color = new Color(150, 255, 0, 255);       
                    break;

                case 8:
                    shakeDuration = 0;
                    shakeMagnitude = 0;
                    foreach (Transform child in ropeSystemGetChild.transform)                                                            
                        child.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0, 255);
                    break;

                default:
                    shakeDuration = 0;
                    shakeMagnitude = 0;
                    foreach (Transform child in ropeSystemGetChild.transform)
                        child.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
                    confirmed = false;
                    break;
            }
        }

        if (num_trig >= num_triggered)
        {
            if (!confirmed && method == MethodToKill.Surround)
            {
                Instantiate(shockwave, transform.position, Quaternion.identity);                        
                confirmed = true;
            }
            if (players[0].Is_Moving() || players[1].Is_Moving())
            {
                /*timerCut += Time.deltaTime;
                if (timerCut > timerCut_TOT)
                {
                    players[0].testVibrationHitRope = true;
                    players[1].testVibrationHitRope = true;
                            
                    GetComponentInParent<Collider2D>().enabled = false;
                    GetComponentInParent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    StartCoroutine(Dead());
                }*/

                for (int i = 0; i < players.Count; i++)
                    players[i].Stop_Moving();
                mashingCanvas.SetActive(true);
            }
            else
                timerCut = 0;
        }
        else
        {
            timerCut = 0;
        }
    }

    void CameraShake()
    {
        if (shakeDuration > 0)        
            cameraTransform.localPosition = initialPosition + UnityEngine.Random.insideUnitSphere * shakeMagnitude;        
        else        
            cameraTransform.localPosition = initialPosition;
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
            players[0].testVibrationHitRope = true;
            players[1].testVibrationHitRope = true;

            sprite.material = flash_sprite;
            sprite.color = Color.white;

            yield return new WaitForSeconds(0.2f);
            sprite.material = default_sprite;
            sprite.color = Color.white;

            yield return new WaitForSeconds(0.2f);   
            Instantiate(blood_explo, new Vector3(transform.position.x, transform.position.y, blood_explo.transform.position.z), blood_explo.transform.rotation);

            yield return new WaitForSeconds(0.25f);
            GetComponentInParent<Animator>().Play("Die");
            for (int i = 0; i < players.Count; i++)
                players[i].Allow_Moving();
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
