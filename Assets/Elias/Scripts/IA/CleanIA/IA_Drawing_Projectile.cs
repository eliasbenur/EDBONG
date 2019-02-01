using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_Drawing_Projectile : MonoBehaviour
{

    [HideInInspector] public List<Transform> allChilds = new List<Transform>();
    [HideInInspector] public List<GameObject> allPlayers = new List<GameObject>();
    [HideInInspector] public GameObject target;

    //Detection's variable, tweekable 
    public float detectionDistance, distanceShoot, speedProjectile;

    //Variable for projectile's shoot, tweekable
    IEnumerator coroutineFire;
    [HideInInspector] public bool canShoot = true;  
    public float cooldown;
    public float cooldownWait;
    public float projectileToFire;

    //Death detection 
    [HideInInspector] public List<encer_trig> list_trig;
    [HideInInspector] public Rope_System rope_system;
    bool dead;
    public float timerCut, timerCut_TOT;
    int num_trig = 0;
    public AudioSource hit_lasser;
    public AudioSource audio_explision;
    public GameObject blood_explo;

    private void Awake()
    {
        foreach(Transform child in transform)
        {
            allChilds.Add(child.transform);
        }

        foreach (Transform child in transform)
        {
            list_trig.Add(child.GetComponent<encer_trig>());
        }
    }

    void Update()
    {
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

        if (target != null)
        {
            if (GetDistance(target) < distanceShoot)
            {
                if (canShoot)
                {
                    coroutineFire = FireCoroutine(cooldown);
                    StartCoroutine(coroutineFire);
                }
            }
        }

        Start_surround();
        if (num_trig >= 3)
        {
            if (allPlayers[0].GetComponent<Player_Movement>().moveX != 0 || allPlayers[0].GetComponent<Player_Movement>().moveY != 0)
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

    IEnumerator FireCoroutine(float cooldown)
    {
        for (int i = 0; i <= projectileToFire; i++)
        {
            foreach (Transform child in allChilds)
            {
                var instanceAddForce = Instantiate(Resources.Load("ShotDistance"), new Vector2(transform.position.x, transform.position.y), Quaternion.identity) as GameObject;
                instanceAddForce.GetComponent<Rigidbody2D>().AddForce((child.transform.position - transform.position) * speedProjectile, ForceMode2D.Impulse);
                //We wait a short time, to let the previous element go more forward before spawing another one 
                canShoot = false;
                yield return new WaitForSeconds(cooldownWait);
                canShoot = true;
            }
        }
        canShoot = false;
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }

    IEnumerator Dead()
    {
        if (!dead)
        {
            dead = true;
            //We use it to control vibrations in both controller
            allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
            allPlayers[1].GetComponent<Player_Movement>().testVibrationHitRope = true;
            if (!hit_lasser.isPlaying)
            {
                hit_lasser.Play();
            }
            audio_explision.Play();
            Instantiate(blood_explo, new Vector3(transform.position.x, transform.position.y, blood_explo.transform.position.z), blood_explo.transform.rotation);
            yield return new WaitForSeconds(0.5f);
        }
        Destroy(this.gameObject);
    }

    float GetDistance(GameObject obj)
    {
        float distance = Vector2.Distance(obj.transform.position, transform.position);
        return distance;
    }
}
