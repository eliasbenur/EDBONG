using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Kamikaza : MonoBehaviour
{
    private List<GameObject> allPlayers = new List<GameObject>();
    private GameObject target;

    bool attack;
    //Tweekable value
    public float detectionDistance;
    public float enemySpeed, oldSpeed;  
    public float timer, timer_BeforeAttack;
    public float timer_BeforeExplosion;
    IEnumerator Kamikaza;
    //Tweekable value for explosion on death
    private float angle;
    public float projectileToSpawn;
    public float angleToADD;
    public float speedProjectile;
    public float cooldown, cooldownToWait;
    bool dieQuick, canShoot;

    //Blincking Effect
    private float spriteBlinkingTimer;
    public float spriteBlinkingMiniDuration;
    public float spriteBlinkingTotalTimer;
    public float spriteBlinkingTotalDuration;
    public bool startBlinking = false;

    bool dead;

    private void Awake()
    {
        oldSpeed = enemySpeed;
    }
    void Update()
    {
        if (startBlinking)
            SpriteBlinkingEffect();
        
            if (dieQuick)
            {

            //GamePad.SetVibration(collision.gameObject.GetComponent<PlayerMovement_E_Modif>().playerIndex, 0,1);
            //collision.gameObject.GetComponent<PlayerMovement_E_Modif>().VibrateRightFull();
            allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
            allPlayers[1].GetComponent<Player2_Movement>().testVibrationHitRope = true;
            /*
            for (int i = 0; i < transform.parent.GetComponent<Rooms>().currentEnnemies.Count; i++)
            {
                if (this.gameObject.transform == transform.parent.GetComponent<Rooms>().currentEnnemies[i])
                    transform.parent.GetComponent<Rooms>().currentEnnemies.RemoveAt(i);
            }
            var coinToDropRand = Random.Range(1, 3);
            var coinCount = 0;
            if (coinCount <= coinToDropRand)
            {
                //Instantiate(coinToDrop, transform.position, Quaternion.identity);
                Instantiate(Resources.Load("CoinAnim"), transform.position, Quaternion.identity);
                coinCount++;
            }
            else
                return;*/
            Destroy(this.gameObject);
        }
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
            if (attack)
            {
                timer += Time.deltaTime;
                if (timer > timer_BeforeAttack)
                {
                    Camera.main.GetComponent<GameManager>().Hit();
                    timer = 0;
                }
            }
            else
            {
                timer = 0;
            }
        }
        //Look at the Target
        if (target != null)
        {
            transform.LookAt(target.transform.position);
            transform.Rotate(new Vector2(0, 90));
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        }
    }

    IEnumerator Wait_EXPLOSIONN(float cooldown)
    {
        if (!dead)
        {
            dead = true;
            GetComponent<CircleCollider2D>().enabled = false;
            detectionDistance = 0;
            enemySpeed = 0;
            startBlinking = true;
            angle = 2 * Mathf.PI;
            dieQuick = false;
            yield return new WaitForSeconds(timer_BeforeExplosion);
            dieQuick = true;
            for (int i = 0; i < projectileToSpawn; i++)
            {
                angle += angleToADD;
                Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
                var instanceAddForce = Instantiate(Resources.Load("ShotDistance"), transform.position + direction, Quaternion.identity) as GameObject;
                var directionVect = instanceAddForce.transform.position - transform.position;
                instanceAddForce.GetComponent<Rigidbody2D>().AddForce(directionVect.normalized * speedProjectile);
            }
        }

    }

    float GetDistance(GameObject obj)
    {
        float distance = Vector2.Distance(obj.transform.position, transform.position);
        return distance;
    }

    void Follow()
    {
        //transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Time.deltaTime * enemySpeed);
        Vector3 Delta = transform.position - target.transform.position;
        gameObject.GetComponent<Rigidbody2D>().MovePosition(transform.position + Delta.normalized * enemySpeed);
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
            attack = true;
        }


        if (collision.gameObject.tag == "rope")
        {
            Kamikaza = Wait_EXPLOSIONN(timer_BeforeExplosion);
            StartCoroutine(Kamikaza);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            enemySpeed = oldSpeed;
            attack = false;
        }
    }

    #endregion

}
