using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_Distance_Shoot_Walk : MonoBehaviour
{
    [HideInInspector] public GameObject target;
    [HideInInspector] public List<GameObject> allPlayers = new List<GameObject>();

    //Detection's variable, tweekable 
    public float detectionDistance, detectionDistance_minimalBeforeLeave, distanceShoot;

    //Variable for projectile's : tweekable
    public float enemySpeed, speedProjectile;
    bool canShoot = true;
    IEnumerator coroutineFire;  
    public float cooldown, cooldown_betweenNextProejctile;
    public float projectileToFire;

    //Variable for death behavior
    bool dead;
    [HideInInspector] public List<encer_trig> list_trig;
    [HideInInspector] public Rope_System rope_system;
    public float timerCut, timerCut_TOT;
    [HideInInspector] public int num_trig = 0;
    [HideInInspector] public AudioSource hit_lasser;
    [HideInInspector] public AudioSource audio_explision;
    [HideInInspector] public GameObject blood_explo;
    public float timer_BeforeExplosion;

    private void Start()
    {
        //We get all childrens -> they all have a script call encer_trig2, he allow us to know if the rope is surrounding him or not
        foreach (Transform child in transform)
        {
            list_trig.Add(child.GetComponent<encer_trig>());
        }
    }

    void Update()
    {
        if (target == null)
        {
            //We detect players, we will not trigger any behavior if players are not here
            foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("player"))
            {
                allPlayers.Add(Obj);
            }
            //Find the closer one -> he will become the target of the monster
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
                    coroutineFire = FireCoroutine(cooldown);
                    StartCoroutine(coroutineFire);
                }
            }
            //But if he's too close, then he leave in the opposite direction of players
            if (GetDistance(target) < detectionDistance_minimalBeforeLeave)
            {
                Leave();
            }
        }

        Start_surround();
        if (num_trig >= 3)
        {
            //If the rope collide with 3 or more then it means that he can be killed
            if (allPlayers[0].GetComponent<Player_Movement>().moveX != 0 || allPlayers[0].GetComponent<Player_Movement>().moveY != 0 /*&&  allPlayers[1].GetComponent<Player2_Movement>().moveX != 0 || allPlayers[1].GetComponent<Player2_Movement>().moveY != 0*/)
            {
                timerCut += Time.deltaTime;
                if (timerCut > timerCut_TOT)
                {
                    //Vibration control for both controller
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

        //Look at the Target
        //No need here ?? Depends on the sprite will be have on this one
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

    IEnumerator Dead()
    {
        if (!dead)
        {
            dead = true;
            if (!hit_lasser.isPlaying)
            {
                hit_lasser.Play();
            }
            audio_explision.Play();
            Instantiate(blood_explo, new Vector3(transform.position.x, transform.position.y, blood_explo.transform.position.z), blood_explo.transform.rotation);
            yield return new WaitForSeconds(timer_BeforeExplosion);
        }
        Destroy(this.gameObject);
    }

    IEnumerator FireCoroutine(float cooldown)
    {
        for (int i = 0; i <= projectileToFire; i++)
        {
            //Projectiles are instantiate and will be targeting the closer player
            var instanceAddForce = Instantiate(Resources.Load("ShotDistance"), new Vector2(transform.position.x, transform.position.y), Quaternion.identity) as GameObject;
            instanceAddForce.GetComponent<Rigidbody2D>().AddForce((target.transform.position - transform.position).normalized * speedProjectile, ForceMode2D.Impulse);
            //We wait a short time, to let the previous element go more forward before spawing an other one 
            canShoot = false;
            yield return new WaitForSeconds(cooldown_betweenNextProejctile);
            canShoot = true;
        }
        canShoot = false;
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }

    //Detect the player and get the distance between them
    float GetDistance(GameObject obj)
    {
        float distance = Vector2.Distance(obj.transform.position, transform.position);
        return distance;
    }

    //If he leave, then he go in the opposite direction of the player
    void Leave()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Time.deltaTime * -enemySpeed);
    }
}
