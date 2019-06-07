using System.Collections;
using UnityEngine;

public class Projectile_Boss_Division : MonoBehaviour
{
    #region Properties
    private bool division;
    public float ennemySpeed;
    private bool canShoot;
    IEnumerator coroutineFire;

    public float angle;
    public float projectileToSpawn;
    public float angleToADD;

    bool confirmed;
    public GameObject shockWave;
    public GameObject shotDistance;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        division = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (division)
        {
            canShoot = true;
            if (!confirmed)
            {
                confirmed = true;
                Instantiate(shockWave, transform.position, Quaternion.identity);
            }
            if (canShoot)
            {
                angle = 2 * Mathf.PI;
                coroutineFire = FireCoroutine_Boss();
                StartCoroutine(coroutineFire);

                AkSoundEngine.PostEvent("play_boss_explodes", Camera.main.gameObject);

                Destroy(this.gameObject);
            }
        }
    }

    IEnumerator FireCoroutine_Boss()
    {
        for (int i = 0; i < projectileToSpawn; i++)
        {
            angle += angleToADD;
            Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            var instance = Instantiate(shotDistance, transform.position + direction, Quaternion.identity);       
            var directionVect = instance.transform.position - transform.position;
            instance.GetComponent<Rigidbody2D>().AddForce(directionVect.normalized * ennemySpeed);
            //A projectile explode in a number of determined projectile in an angle all around him
        }
        canShoot = false;
        division = false;
        yield return null;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //The boss has a circle trigger all around him, when the projectile exit it, then he can be divide
        if (collision.GetType() == typeof(CircleCollider2D))
        {
            if (collision.gameObject.name == "Boss")
            {
                if (collision.tag == "Boss")
                    division = true;
            }
        }       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "WallRooms")
            //If the projectiles touch a wall of the rooms then we make him disappear
            Destroy(this.gameObject);
    }
}
