using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collant_trig : MonoBehaviour{

    float delay;
    float cur_delay;

    private void Start()
    {
        delay = 1f;
        cur_delay = 1f;
    }

    private void Update()
    {
        if (transform.parent.GetComponent<AI_collant>().Player_dashing() && cur_delay >= delay)
        {
            cur_delay = 0;
        }

        if (cur_delay < delay)
        {
            cur_delay += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cur_delay >= delay)
        {
            if (collision.gameObject.layer == 9)
            {
                collision.gameObject.GetComponent<Rope_Point>().enemie_coll = true;
                if (transform.parent.GetComponent<AI_collant>().point_to_coll == null)
                {
                    transform.parent.GetComponent<AI_collant>().point_to_coll = collision.gameObject;
                    transform.parent.GetComponent<CircleCollider2D>().isTrigger = true;
                }
            }
        }
        else
        {
            transform.parent.GetComponent<AI_collant>().point_to_coll = null;
            transform.parent.GetComponent<CircleCollider2D>().isTrigger = false;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            collision.gameObject.GetComponent<Rope_Point>().enemie_coll = false;
            //transform.parent.GetComponent<AI_collant>().point_to_coll = null;
        }
    }
}
