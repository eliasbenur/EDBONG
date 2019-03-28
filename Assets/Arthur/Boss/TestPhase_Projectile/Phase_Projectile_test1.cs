using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase_Projectile_test1 : MonoBehaviour
{
    public bool canShoot;
    IEnumerator coroutineFire;
    public float cooldown_Between_Projectil, projectLeft, cooldown, enemyStart;
    public Vector3 position;

    public int timeToDo;

    // Update is called once per frame
    void Update()
    {
        if (canShoot)
        {
            coroutineFire = FireCoroutine_Boss();
            StartCoroutine(coroutineFire);
        }
    }

    IEnumerator FireCoroutine_Boss()
    {
        for (int j = 0; j < timeToDo; j++)
        {
            for (int i = 0; i < projectLeft; i++)
            {
                var instanceAddForce = Instantiate(Resources.Load("triangleShot"), new Vector2(transform.position.x, transform.position.y), Quaternion.identity) as GameObject;

                position = Random.insideUnitSphere * 10 + transform.position;
                instanceAddForce.GetComponent<Rigidbody2D>().AddForce((position - instanceAddForce.transform.position).normalized * enemyStart);

                var test = position - instanceAddForce.transform.position;

                float rot_z = Mathf.Atan2(test.y, test.x) * Mathf.Rad2Deg;
                instanceAddForce.transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);

                canShoot = false;
                yield return new WaitForSeconds(cooldown_Between_Projectil);
                canShoot = true;
            }
            canShoot = false;
            yield return new WaitForSeconds(cooldown);
            canShoot = true;
        }
        canShoot = false;
    }
}
