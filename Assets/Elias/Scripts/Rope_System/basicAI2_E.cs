using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicAI2_E : MonoBehaviour
{
    public List<GameObject> allPlayers = new List<GameObject>();
    public float distancePreview;
    public GameObject target;
    public float enemySpeed;
    private float previousEnemySpeed;

    private bool targetChange;
    private bool targetChanged;
    private bool multipleColliding;

    public Animator animator;

    public bool trig_left, trig_right, trig_down, trig_up;

    private void Awake()
    {
        if (allPlayers.Count == 0)
        {
            Choice();
        }
        previousEnemySpeed = enemySpeed;
    }


    // Use this for initialization
    void Start()
    {
        //Debug.Log(distancePreview);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target.transform.position);
        transform.Rotate(new Vector2(0, 90));
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);

        if (GetDistance(target) < distancePreview)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Time.deltaTime * enemySpeed);
            animator.SetBool("running", true);
        }
        else
        {
            animator.SetBool("running", false);
        }


        if (targetChange)
            ChangeTargetTrigger();

        #region ToVERIFY
        //if both player are colliding in same time with the monster, monster in default don't know wich one to pick
        //In this case the monster keep 0 in speed and will stay fix until a player collide with him again
        //so we have to plan this case, to avoid some static AI, we find wich one is the closest, and will be the next target
        //when both of them will stop colliding with him
        //Of course, as long are they keep colliding with him, they will loose life
        /*
        if(CollidedObjects.Count ==2)
        {
            multipleColliding = true;
        }
        */
        #endregion

        if (trig_down && trig_left && trig_right && trig_up)
        {
            animator.SetBool("dead", true);
            GetComponent<CircleCollider2D>().enabled = false;
            StartCoroutine(Dead());
        }
    }

    private void Choice()
    {
        foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("player"))
        {
            allPlayers.Add(Obj);
            //Find which one is the closest 
            if (GetDistance(Obj) < distancePreview)
            {
                distancePreview = GetDistance(Obj);
                target = Obj;
            }
        }
        //The enemy is focus on the closest player but we do a lottery draw to add some challenge/ variation, AI has 20% of luck to change his target
        RandomProbTarget();
    }

    float GetDistance(GameObject obj)
    {
        float distance = Vector2.Distance(obj.transform.position, transform.position);
        return distance;
    }

    private void RandomProbTarget()
    {
        int prob = UnityEngine.Random.Range(0, 5);
        switch (prob)
        {
            case 0:
                //80% prob so we won't change anything
                break;

            case 1:
                //80% prob so we won't change anything
                break;

            case 2:
                //80% prob so we won't change anything
                break;

            case 3:
                //80% prob so we won't change anything
                break;

            case 4:
                //20% of success to change the main target
                NewTarget();
                break;
            default:
                break;
        }
    }

    private void ChangeTargetTrigger()
    {
        NewTarget();
        enemySpeed = 0;
        targetChanged = true;
        targetChange = false;
    }

    private void NewTarget()
    {
        if (target == allPlayers[0])
        {
            //Debug.Log("Old Target " + target);
            target = allPlayers[1];
            distancePreview = GetDistance(target);
            //Debug.Log("New target" + target);
        }
        else
        {
            //Debug.Log("Old Target " + target);
            target = allPlayers[0];
            distancePreview = GetDistance(target);
            //Debug.Log("New target" + target);
        }
    }

    //When an enemy collide with a player, he stop moving to avoid some shakings 
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject == target)
        {
            enemySpeed = 0;
        }

        if (collision.gameObject != target && !targetChanged && collision.gameObject.tag == "tack")
        {
            targetChange = true;
        }

        /*if (collision.transform.tag != "player" && collision.transform.tag != "monster")
        {
            if (collision.transform.parent.transform.parent.tag == "rope")
            {
                animator.SetBool("dead", true);
                GetComponent<CircleCollider2D>().enabled = false;
                StartCoroutine(Dead());
            }
        }*/

    }

    IEnumerator Dead()
    {
        distancePreview = float.MaxValue;
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == target)
        {
            enemySpeed = previousEnemySpeed;
            targetChanged = false;
        }

        if (collision.gameObject != target)
        {
            targetChange = false;
            targetChanged = false;
        }

        if (collision.gameObject.tag == "Rope")
        {
            enemySpeed = previousEnemySpeed;
        }
    }
}
