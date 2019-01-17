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

    public List<encer_trig2> list_trig;

    public Rope_System rope_system;
    public bool rope_atachment;

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
        foreach(Transform child in transform)
        {
            list_trig.Add(child.GetComponent<encer_trig2>());
        }
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

        /*if (trig_down && trig_left && trig_right && trig_up)
        {
            animator.SetBool("dead", true);
            GetComponent<CircleCollider2D>().enabled = false;
            StartCoroutine(Dead());
        }*/

        start_surround();
    }

    void start_surround()
    {
        int num_trig = 0;
        foreach (encer_trig2 trig in list_trig)
        {
            if (trig.check_isTouching())
            {
                num_trig++;
            }
        }
        if (rope_atachment)
        {
            if (num_trig >= 4 && !player_echap())
            {
                rope_system.state_surround = true;
            }
            else
            {
                rope_system.state_surround = false;
            }
        }
        else
        {
            rope_system.state_surround = false;
        }

    }

    public bool player_echap()
    {
        Vector3 ligne_echappe = allPlayers[0].transform.position - allPlayers[1].transform.position;
        Vector3 player_dir = allPlayers[0].GetComponent<Player_Movement>().movement.normalized + allPlayers[1].GetComponent<Player2_Movement>().movement.normalized;
        Vector3 playerto_enemy = transform.position - allPlayers[1].transform.position;
        if (player_dir == Vector3.zero)
        {
            return false;
        }
        if (Vector2.Dot(Vector2.Perpendicular(ligne_echappe.normalized), playerto_enemy.normalized) > 0) // BAS
        {
            if (Vector2.Dot(Vector2.Perpendicular(ligne_echappe.normalized), player_dir.normalized) < 0)
            {
                LayerMask lm = LayerMask.GetMask("Rope");
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.Perpendicular(ligne_echappe.normalized), 2, lm);
                Debug.DrawLine(transform.position, transform.position + (Vector3)Vector2.Perpendicular(ligne_echappe.normalized), Color.red, 10f);
                if (hit.collider != null)
                {
                    Debug.Log("Encer");
                    return false;
                }
                Debug.Log("Echap");
                return true;
            }
            else
            {
                Debug.Log("Echap");
                return true;
            }
        }
        else // UP
        {
            if (Vector2.Dot(Vector2.Perpendicular(ligne_echappe.normalized), player_dir.normalized) > 0)
            {
                LayerMask lm = LayerMask.GetMask("Rope");
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.Perpendicular(ligne_echappe.normalized), 2, lm);
                Debug.DrawLine(transform.position, transform.position + (Vector3)Vector2.Perpendicular(ligne_echappe.normalized), Color.red, 10f);
                if (hit.collider != null)
                {
                    Debug.Log("Encer");
                    return false;
                }
                Debug.Log("Echap");
                return true;
            }
            else
            {
                Debug.Log("Echap");
                return true;
            }
        }
        //Debug.Log(Vector2.Dot(Vector2.Perpendicular(ligne_echappe.normalized), player_dir.normalized));
        //return true;
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
