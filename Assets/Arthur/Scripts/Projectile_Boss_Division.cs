using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Boss_Division : MonoBehaviour
{
    public bool division;
    public float ennemySpeed;//WARNING : this variable should have the same value has the previous projectile's speed
    bool canShoot;
    IEnumerator coroutineFire;

    public float angle;
    public float projectileToSpawn;
    public float angleToADD;
    //public float timer, timerTOT;


    // Start is called before the first frame update
    void Awake()
    {
        division = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(division)
        {
            canShoot = true;
        }
        /*
        timer += Time.deltaTime;
        if (timer > timerTOT)
        {
            division = true;
            canShoot = true;
            timer = 0;
        }*/

        if (division && canShoot)
        {
            angle = 2 * Mathf.PI;
            coroutineFire = FireCoroutine_Boss();
            StartCoroutine(coroutineFire);
            Destroy(this.gameObject);
        }
    }

    IEnumerator FireCoroutine_Boss()
    {
        for (int i = 0; i < projectileToSpawn; i++)
        {
            angle += angleToADD;
            Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            var instanceAddForce = Instantiate(Resources.Load("ShotDistance"),transform.position + direction, Quaternion.identity) as GameObject;
            var directionVect = instanceAddForce.transform.position - transform.position;
            instanceAddForce.GetComponent<Rigidbody2D>().AddForce( directionVect.normalized * ennemySpeed);
            //We wait a short time, to let the previous element go more forward before spawing an other one 
            canShoot = true;
        }
        canShoot = false;
        division = false;
        yield return null;      
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag =="Boss")
            division = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "WallRooms")
        {
            Destroy(this.gameObject);
        }
    }
}
