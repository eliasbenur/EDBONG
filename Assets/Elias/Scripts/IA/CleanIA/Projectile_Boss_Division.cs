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
            //A projectile explode in a number of determined projectile in an angle all around him
        }
        canShoot = false;
        division = false;
        yield return null;      
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag =="Boss")
            division = true;
        //The boss has a circle trigger all around him, when the projectile exit it, then he can be divide
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "WallRooms")
        {
            //If the projectiles touch a wall of the rooms then we make him disappear
            Destroy(this.gameObject);
        }
    }
}
