using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_Distance_Shoot_Walk : MonoBehaviour
{
    [HideInInspector]
    public GameObject target;
    public List<GameObject> allPlayers = new List<GameObject>();
    //Detection's variable, tweekable 
    public float detectionDistance, detectionDistance_minimalBeforeLeave, distanceShoot;
    //Variable for projectile's speed, tweekable
    public float enemySpeed, speedProjectile;
    //Variable for projectile's shoot, tweekable
    bool canShoot = true;
    IEnumerator coroutineFire;  
    public float cooldown, cooldown_betweenNextProejctile;
    public float projectileToFire;

    bool dead;
    public List<encer_trig> list_trig;
    public Rope_System rope_system;
    public bool rope_atachment;

    public float timerCut, timerCut_TOT;

    public int num_trig = 0;
    public float delay_spawn;

    public AudioSource hit_lasser;
    public AudioSource audio_explision;

    public GameObject blood_explo;
    public float timer_BeforeExplosion;

    private void Start()
    {
        foreach (Transform child in transform)
        {
            list_trig.Add(child.GetComponent<encer_trig>());
        }
    }

    void Update()
    {
        if (/*transform.parent.GetComponent<Rooms>().stayedRoom &&*/ target == null)
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
            if (GetDistance(target) < distanceShoot)
            {
                if(canShoot)
                {
                    coroutineFire = FireCoroutine(cooldown);
                    StartCoroutine(coroutineFire);
                }                
            }
            if (GetDistance(target) < detectionDistance_minimalBeforeLeave)
            {
                Leave();
            }
        }

        Start_surround();
        if (num_trig >= 3)
        {
            if (allPlayers[0].GetComponent<Player_Movement>().moveX != 0 || allPlayers[0].GetComponent<Player_Movement>().moveY != 0 /*&&  allPlayers[1].GetComponent<Player2_Movement>().moveX != 0 || allPlayers[1].GetComponent<Player2_Movement>().moveY != 0*/)
            {
                timerCut += Time.deltaTime;
                if (timerCut > timerCut_TOT)
                {
                    allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
                    allPlayers[1].GetComponent<Player_Movement>().testVibrationHitRope = true;
                    GetComponent<CircleCollider2D>().enabled = false;
                    StartCoroutine(Dead());

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
        //No need here ?? Depends on the sprite will be have on this one
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
        if (!dead && delay_spawn <= 0)
        {
            dead = true;
            allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
            allPlayers[1].GetComponent<Player_Movement>().testVibrationHitRope = true;
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

    float GetDistance(GameObject obj)
    {
        float distance = Vector2.Distance(obj.transform.position, transform.position);
        return distance;
    }
    void Leave()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Time.deltaTime * -enemySpeed);
    }

    #region Fonction void ON
    #endregion
}
