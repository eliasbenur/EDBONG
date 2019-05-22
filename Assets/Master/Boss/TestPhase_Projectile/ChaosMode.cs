using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosMode : MonoBehaviour
{
    public float ennemySpeed;//WARNING : this variable should have the same value has the previous projectile's speed
    public bool canShoot = true;
    IEnumerator coroutineFire;

    public float angle;
    public float projectileToSpawn;
    public float angleToADD;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (canShoot)
        {
            angle = 2 * Mathf.PI;
            coroutineFire = FireCoroutine_Boss();
            StartCoroutine(coroutineFire);
        }
    }

    IEnumerator FireCoroutine_Boss()
    {
        for (int i = 0; i < projectileToSpawn; i++)
        {
            angle += angleToADD;
            Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            var instanceAddForce = Instantiate(Resources.Load("ShotDistance"), transform.position + direction, Quaternion.identity) as GameObject;
            var directionVect = instanceAddForce.transform.position - transform.position;
            instanceAddForce.GetComponent<Rigidbody2D>().AddForce(directionVect.normalized * ennemySpeed);
            //A projectile explode in a number of determined projectile in an angle all around him
        }
        canShoot = false;
        yield return null;
    }
}
