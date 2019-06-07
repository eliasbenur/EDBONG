using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_tp : MonoBehaviour{

    #region Properties

    public float delay_tp;
    float curr_delay_tp;
    public List<Transform> targets;
    public GameObject target;
    private Rope_System rope_system;
    private List<Player_Movement> players;
    public float distanceShoot;
    IEnumerator coroutineFire;
    public float cooldown, cooldown_betweenNextProejctile;
    bool dead = false;
    public float projectileToFire;
    public float speedProjectile;
    int num_trig = 0;
    private float shakeDuration;
    private float shakeMagnitude;
    GameObject ropeSystemGetChild;
    bool confirmed;

    private RippleEffect shockwave;

    private Vector3 initialPosition;
    private Transform cameraTransform;
    [HideInInspector] public List<encer_trig> list_trig;
    float num_triggered;
    float timerCut_TOT;
    Animator animator;
    float timerCut;
    public GameObject blood_explo;
    private moneyDrop moneyDrop;
    private bool tp_active = false;

    #region SPAWN / General Behavior
    private Blinking_Effect blink;
    bool canShoot = true;
    #endregion


    #endregion

    private void Awake()
    {
        shockwave = Camera.main.GetComponent<RippleEffect>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rope_system = GameObject.Find("Rope_System").GetComponent<Rope_System>();
        blink = GetComponent<Blinking_Effect>();
        players = Camera.main.GetComponent<GameManager>().players_Movement;
        ropeSystemGetChild = rope_system.gameObject;
        cameraTransform = Camera.main.GetComponent<Transform>();
        moneyDrop = GetComponent<moneyDrop>();

        foreach (Transform child in transform)
        {
            if (child.name != "CleanCollision")
                list_trig.Add(child.GetComponent<encer_trig>());
        }

        num_triggered = 8;
        timerCut_TOT = 0.7f;

        animator = GetComponent<Animator>();

        curr_delay_tp = delay_tp;
        targets.Clear();
        targets.Add(rope_system.get_points()[0].transform);
        targets.Add(rope_system.get_points()[rope_system.NumPoints - 1].transform);

    }

    // Update is called once per frame
    void Update()
    {
        if (blink.spawn)
            blink.SpriteBlinkingEffect();
        else
        {

            // TP
            if (curr_delay_tp > 0)
            {
                curr_delay_tp -= Time.deltaTime;
            }
            else
            {
                if ((targets[0].transform.position - transform.position).magnitude < 5 || (targets[1].transform.position - transform.position).magnitude < 5)
                {
                    tp_active = true;
                    StartCoroutine("Tepe");
                    curr_delay_tp = delay_tp;

                }
            }

            //If the monster don't have target then we look for one
            if (target == null)
            {
                var maxDistance = float.MaxValue;
                for (int i = 0; i < players.Count; i++)
                {
                    var whichOneCloser = GetDistance(players[i].gameObject);
                    if (whichOneCloser < maxDistance)
                    {
                        target = players[i].gameObject;
                        maxDistance = whichOneCloser;
                    }
                }
            }

            //SHOOT
            if (target != null && !tp_active)
            {
                //If one player (who are not the actual target) is closer than the target, then the script change of target
                var maxDistance = float.MaxValue;
                for (int i = 0; i < players.Count; i++)
                {
                    var whichOneCloser = GetDistance(players[i].gameObject);
                    if (whichOneCloser < maxDistance)
                    {
                        target = players[i].gameObject;
                        maxDistance = whichOneCloser;
                    }
                }
                //Condition to turn animations on
                if (GetDistance(target) < distanceShoot)
                {
                    Debug.Log("Dist MAX ok");
                    if (canShoot)
                    {
                        if (GetDistance(target) > 4)
                        {
                            Debug.Log("Dist MIN ok");
                            coroutineFire = FireCoroutine(cooldown);
                            StartCoroutine(coroutineFire);
                        }
                    }
                }
            }

            Start_surround();
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

            if (num_trig >= num_triggered && !tp_active)
            {
                if (Players_dashing())
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

                if (players[0].get_MovementX() != 0 || players[1].get_MovementY() != 0)
                {
                    timerCut += Time.deltaTime;
                    transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.6f, 1, 1), timerCut / timerCut_TOT);

                    if (timerCut > timerCut_TOT)
                    {
                        players[0].testVibrationHitRope = true;
                        players[1].testVibrationHitRope = true;

                        gameObject.GetComponent<Collider2D>().enabled = false;
                        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                        transform.localScale = new Vector3(1, 1, 1);

                        animator.SetBool("dead", true);
                        StartCoroutine(Dead());

                    }
                }
                else
                    timerCut = 0;
            }
            else
            {
                timerCut = 0;
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    public bool Players_dashing()
    {
        return (players[0].Dashing() || players[1].Dashing());
    }

    IEnumerator Dead()
    {
        if (!dead)
        {
            dead = true;
            AkSoundEngine.PostEvent("plays_slicing", Camera.main.gameObject);
            AkSoundEngine.PostEvent("play_monster1death", Camera.main.gameObject);

            animator.SetBool("dead", true);

            blink.SpriteBlinkingEffect();
            //Used to control the vibrations in both controllers
            players[0].testVibrationHitRope = true;
            players[1].testVibrationHitRope = true;

            GetComponent<SpriteRenderer>().color = Color.white;

            yield return new WaitForSeconds(0.2f);
            GetComponent<SpriteRenderer>().color = Color.white;

            GetComponent<SpriteRenderer>().color = Color.white;
            var newPosition = Camera.main.WorldToScreenPoint(transform.position);
            newPosition = new Vector3(newPosition.x / Screen.width, newPosition.y / Screen.height);
            shockwave.Emit(newPosition.x, newPosition.y);

            yield return new WaitForSeconds(0.2f);

            Instantiate(blood_explo, new Vector3(transform.position.x, transform.position.y, blood_explo.transform.position.z), blood_explo.transform.rotation);
            if (moneyDrop != null)
                moneyDrop.enabled = true;
            yield return new WaitForSeconds(0.25f);

            Destroy(gameObject);
        }
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

    void Start_surround()
    {
        num_trig = 0;
        foreach (encer_trig trig in list_trig)
        {
            if (trig.Check_isTouching())
                num_trig++;
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


    float GetDistance(GameObject obj)
    {
        float distance = Vector2.Distance(obj.transform.position, transform.position);
        return distance;
    }

    IEnumerator Tepe()
    {
        animator.SetBool("tp", true);
        yield return new WaitForSeconds(1.875f);
        animator.SetBool("tp", false);
        tp_active = false;
        Vector3 new_pos = GetCenterPoint();
        Vector3 dir = new_pos - transform.position;
        float angle_new_pos = Vector2.SignedAngle(Vector2.right, dir);

        float angle = Random.Range(-45, 45);
        angle = (angle_new_pos + angle) * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);

        LayerMask lm = LayerMask.GetMask("Walls", "Door");
        RaycastHit2D hit = Physics2D.Raycast(new_pos, direction, 15, lm);

        if (hit.collider != null)
        {
            transform.position = new_pos + direction.normalized * (hit.distance -2);
        }
        else
        {
            transform.position = new_pos + direction.normalized * 15;
        }
    }

    Vector3 GetCenterPoint()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }

    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
