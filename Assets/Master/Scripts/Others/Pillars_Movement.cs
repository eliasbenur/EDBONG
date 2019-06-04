using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillars_Movement : MonoBehaviour
{
    public Transform posA, posB;
    private bool posA_active = true;
    public float speed;
    
    void FixedUpdate()
    {
        if (posA_active)
        {
            transform.position += (posA.position - transform.position) * Time.fixedDeltaTime * speed;
            //gameObject.GetComponent<Rigidbody2D>().MovePosition(transform.position + (posA.position - transform.position) * Time.fixedDeltaTime * speed);
        }
        else
        {
            transform.position += (posB.position - transform.position) * Time.fixedDeltaTime * speed;
            //gameObject.GetComponent<Rigidbody2D>().MovePosition(transform.position + (posB.position - transform.position) * Time.fixedDeltaTime * speed);
        }


        if (Vector3.Distance(transform.position, posA.position) < 1)
        {
            posA_active = false;
        }

        if (Vector3.Distance(transform.position, posB.position) < 1)
        {
            posA_active = true;
        }
    }

}
