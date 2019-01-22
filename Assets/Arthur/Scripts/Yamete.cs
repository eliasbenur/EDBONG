using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yamete : MonoBehaviour
{
    [HideInInspector]
    public List<Transform> allChilds = new List<Transform>();
    public List<GameObject> allPlayers = new List<GameObject>();
    public GameObject target;
    //Detection's variable, tweekable 
    public float detectionDistance, distanceShoot, speedProjectile;    

    bool hit;
    //Variable for projectile's shoot, tweekable
    public bool canShoot = true;
    IEnumerator coroutineFire;
    public float cooldown;
    public float cooldownWait;
    public float projectileToFire;

    private void Awake()
    {
        foreach(Transform child in transform)
        {
            allChilds.Add(child.transform);
        }
    }
    void Update()
    {
        if (hit)
            Camera.main.GetComponent<GameManager>().Hit();
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
            if (GetDistance(target) < distanceShoot)
            {
                if (canShoot)
                {
                    coroutineFire = FireCoroutine(cooldown);
                    StartCoroutine(coroutineFire);
                }
            }
        }
        //Look at the Target
        //No need here ?? Depends on the sprite will be have on this one
        if (target != null)
        {
            transform.LookAt(target.transform.position);
            transform.Rotate(new Vector2(0, 90));
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
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

    float GetDistance(GameObject obj)
    {
        float distance = Vector2.Distance(obj.transform.position, transform.position);
        return distance;
    }

    #region Fonction void ON

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "rope")
        {
            //GamePad.SetVibration(collision.gameObject.GetComponent<PlayerMovement_E_Modif>().playerIndex, 0,1);
            //collision.gameObject.GetComponent<PlayerMovement_E_Modif>().VibrateRightFull();
            allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
            allPlayers[1].GetComponent<Player2_Movement>().testVibrationHitRope = true;

            /*for (int i = 0; i < transform.parent.GetComponent<Rooms>().currentEnnemies.Count; i++)
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
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            hit = false;
        }
    }

    #endregion
}
