using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_IA_Collant : MonoBehaviour
{
    //Distance limit where players can be detected
    public float detectionDistance;
    //Movement speed of the monster
    public float enemySpeed;
    //Not use for now, -> time before the ennemy's ready to attack
    public float delay_Action;

    public GameObject point_to_coll;

    public Initialisation_Rope init_IA;
    public Detection_dash_Distance checkDistance;

    private void Awake()
    {
        init_IA = GetComponent<Initialisation_Rope>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //We don't use it for now, can be used to have a delay on the monster's spawn
        if (delay_Action > 0)
        {
            delay_Action -= Time.deltaTime;
        }
        else
        {
            if (init_IA.target != null)
            {
                if (checkDistance.GetDistance(init_IA.target) < detectionDistance)
                {
                    Follow();
                }
            }
        }

        if (/*transform.parent.GetComponent<Rooms>().stayedRoom && */init_IA.target == null)
        {
            foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("player"))
            {
                init_IA.allPlayers.Add(Obj);
            }
            if (init_IA.rope_system.Points.Count > 0)
            {
                init_IA.target = init_IA.rope_system.Points[init_IA.rope_system.NumPoints / 2].gameObject;
            }
        }
    }

    void Follow()
    {
        if (point_to_coll != null)
        {
            transform.position = point_to_coll.transform.position;
        }
        else
        {
            Vector3 Delta = init_IA.target.transform.position - transform.position;
            gameObject.GetComponent<Rigidbody2D>().MovePosition(transform.position + Delta.normalized * Time.fixedDeltaTime * enemySpeed);
        }
    }
}
