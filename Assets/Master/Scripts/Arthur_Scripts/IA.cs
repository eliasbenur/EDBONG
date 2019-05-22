using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class IA : MonoBehaviour
{
    private List<GameObject> allPlayers = new List<GameObject>();
    private GameObject target;
    //Tweekable value
    public float detectionDistance;
    public float enemySpeed, oldSpeed;
    bool attack;
    public float timer, timer_BeforeAttack;

    private void Awake()
    {
        oldSpeed = enemySpeed;
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

        if(target != null)
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
            if(attack)
            {
                timer += Time.deltaTime;
                if (timer > timer_BeforeAttack)
                {
                    Camera.main.GetComponent<GameManager>().Hit_verification("PlayerUndefined", transform.position, "IA");
                    timer = 0;                
                }
            }
            else
            {
                timer = 0;
            }
        }           
        //Look at the Target
        if(target != null)
        {
            transform.LookAt(target.transform.position);
            transform.Rotate(new Vector2(0, 90));
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
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
    }

    #region Fonction void ON

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            enemySpeed = 0;
            attack = true;
        }


        /*if (collision.gameObject.tag == "rope")
        {
            //GamePad.SetVibration(collision.gameObject.GetComponent<PlayerMovement_E_Modif>().playerIndex, 0,1);
            //collision.gameObject.GetComponent<PlayerMovement_E_Modif>().VibrateRightFull();
            target.GetComponent<PlayerMovement_E_Modif>().testVibrationHitRope = true;
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
                return;
            Destroy(this.gameObject);
        }*/
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
