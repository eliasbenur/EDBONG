using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_Fire_Target : MonoBehaviour
{
    //Detection of targets
    public float detectionDistance, leaveDetectionDistance;
    //List and current target selected
    private List<GameObject> allPlayers = new List<GameObject>();   
    [HideInInspector]
    public GameObject target;
    //moving variables of the target sprite and the object
    public float enemySpeed, enemySpeedLeaving, previousSpeed;
    //Coroutine and cooldown variable for the shoot
    IEnumerator coroutineFire;
    bool canShoot;
    //Time to wait before the projectile is fired
    public float timer, timer_BeforeShooting;
    //Method to change : list of the collider actually colliding with the rope, to detect if we are surrounding the ennemy
    //public List<CircleCollider2D> colliderRopeSurround;
    //public int count;

    private void Awake()
    {
        foreach(Transform child in transform)
        {
            if (child.name == "target")
                continue;
            //colliderRopeSurround.Add(child.GetComponent<CircleCollider2D>());
        }
        previousSpeed = enemySpeedLeaving;
    }

    void Update()
    {
        //TODO: Change the method of surrounding with the Elia's one
        /*for (int i = 0; i < colliderRopeSurround.Count; i++)
        {
            if (colliderRopeSurround[i].GetComponent<checkSurroundRope>().trigger && !colliderRopeSurround[i].GetComponent<checkSurroundRope>().check)
            {
                colliderRopeSurround[i].GetComponent<checkSurroundRope>().check = true;
                count +=1;
            }
            else if(!colliderRopeSurround[i].GetComponent<checkSurroundRope>().trigger && colliderRopeSurround[i].GetComponent<checkSurroundRope>().check)
            {
                colliderRopeSurround[i].GetComponent<checkSurroundRope>().check = false;
                count -= 1;
            }
        }
        if(count > 6)
        {
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

        //if the player is touch by a projectile, we trigger the hit's fonction for the screen shake, controller vibration...
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
            if (GetDistance(target) < detectionDistance)
            {
                Attack();
            }    
            if (GetDistance(target) < leaveDetectionDistance)
            {
                Leave();
            }
            foreach (Transform child in transform)
            {
                if (child.transform.position == target.transform.position)
                {
                    timer += Time.deltaTime;
                    if (timer > timer_BeforeShooting)
                    {
                        canShoot = true;
                        if (canShoot)
                        {
                            coroutineFire = FireCoroutine(5);
                            StartCoroutine(coroutineFire);
                            timer = 0;
                        }
                    }
                }
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

    float GetDistance(GameObject obj)
    {
        float distance = Vector2.Distance(obj.transform.position, transform.position);
        return distance;
    }

    void Attack()
    {
        foreach (Transform child in transform)
        {
            if(child.name == "target")
            {
                child.GetComponent<Transform>().position = Vector2.MoveTowards(child.GetComponent<Transform>().position, target.transform.position, Time.deltaTime * enemySpeed);
                child.gameObject.SetActive(true);
            }        
        }
    }

    void Leave()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Time.deltaTime * -enemySpeedLeaving);
        foreach (Transform child in transform)
        {
            if (child.name == "target")
            {
                child.GetComponent<Transform>().position = transform.position;
                child.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator FireCoroutine(float cooldown)
    {
        Instantiate(Resources.Load("projectile_Fire_IA"), new Vector2(target.transform.position.x, target.transform.position.y + 80), Quaternion.identity, transform);
        canShoot = false;
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }
    #region Fonction void ON:
    //Fonction void ON:
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "rope")
        {
            enemySpeedLeaving = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "rope")
        {
            enemySpeedLeaving = previousSpeed;
        }
    }
    #endregion
}
