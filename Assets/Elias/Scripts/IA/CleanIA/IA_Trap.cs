using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_Trap : MonoBehaviour
{
    //Detect all the players, we'll need it for the vibration control, or even to not trigger behavior if players are not here
    public List<GameObject> allPlayers = new List<GameObject>();
    public GameObject target;
    public float detectionDistance;
    //Rope system will be used to found a target for the monster
    public Rope_System rope_system;
    public float enemySpeed;

    public Animator animator;
    bool dead;

    //Trou is trap -> the only way to kill this monster
    //We use it to center the position of the monster in the middle of the trap when is "falling"
    GameObject trou;

    void Start()
    {
        dead = false;
        if (rope_system == null)
        {
            rope_system = GameObject.Find("Rope_System").GetComponent<Rope_System>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {
            if (GetDistance(target) < detectionDistance)
            {
                Follow();
                //animator.SetBool("running", true);
            }
            else
            {
                //animator.SetBool("running", false);
            }
        }

        if (target == null)
        {
            foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("player"))
            {
                allPlayers.Add(Obj);
            }
            if (rope_system.Points.Count > 0)
            {
                target = rope_system.Points[rope_system.NumPoints / 2].gameObject;
            }
        }

        if (dead)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.8f * Time.fixedDeltaTime);
            transform.position = Vector3.Lerp(transform.position, trou.transform.position, 2f * Time.fixedDeltaTime);
            //transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(0,0, Random.Range(30,600),0),Time.fixedDeltaTime * 0.01f);
        }
    }

    float GetDistance(GameObject obj)
    {
        float distance = Vector2.Distance(obj.transform.position, transform.position);
        return distance;
    }

    void Follow()
    {
        if (enemySpeed != 0)
        {
            Vector3 Delta = target.transform.position - transform.position;
            gameObject.GetComponent<Rigidbody2D>().MovePosition(transform.position + Delta.normalized * Time.fixedDeltaTime * enemySpeed);
        }
    }

    //When an enemy collide with a player, he stop moving to avoid some shakings 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            allPlayers[0].GetComponent<Player_Movement>().alreadyVibrated = false;
            allPlayers[1].GetComponent<Player_Movement>().alreadyVibrated = false;
        }
    }

    IEnumerator Dead()
    {
        if (!dead)
        {
            dead = true;         
            allPlayers[0].GetComponent<Player_Movement>().testVibrationHitRope = true;
            allPlayers[1].GetComponent<Player_Movement>().testVibrationHitRope = true;
            enemySpeed = 0;
            gameObject.GetComponent<CircleCollider2D>().isTrigger = true;
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "piege")
        {
            trou = collision.gameObject;
            StartCoroutine(Dead());
        }
    }
}
