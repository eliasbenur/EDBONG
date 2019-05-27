using UnityEngine;

public class Collant_trig : MonoBehaviour{

    #region Properties
    private float delay;
    private float cur_delay;

    private Detection_dash_Distance dash_Check;
    private Movement_IA_Collant check_point;
    private new CircleCollider2D collider;
    #endregion

    private void Awake()
    {
        dash_Check = transform.parent.GetComponent<Detection_dash_Distance>();
        check_point = transform.parent.GetComponent<Movement_IA_Collant>();
        collider = transform.parent.GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        delay = 1f;
        cur_delay = 1f;
    }

    private void Update()
    {
        if (dash_Check.Player_dashing() && cur_delay >= delay)        
            cur_delay = 0;        

        if (cur_delay < delay)        
            cur_delay += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If players are dashing and the monster is stick to the chain
        if (cur_delay >= delay)
        {
            if (collision.gameObject.layer == 9)
            {
                //Then we takes off, and in delay's value, he would not be able to stick again
                collision.gameObject.GetComponent<Rope_Point>().enemie_coll = true;
                if (check_point.point_to_coll == null)
                {
                    check_point.point_to_coll = collision.gameObject;
                    collider.isTrigger = true;
                }
            }
        }
        else
        {
            check_point.point_to_coll = null;
            collider.isTrigger = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
            collision.gameObject.GetComponent<Rope_Point>().enemie_coll = false;
    }
}

