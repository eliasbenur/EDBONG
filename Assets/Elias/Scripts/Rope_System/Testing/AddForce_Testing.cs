using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForce_Testing : MonoBehaviour {

    public Transform parent;
    public Transform parent2;
    Rigidbody2D rb2D;
    public float sens;

    Vector2 movement;


    public float speed, moveX, moveY;
    public string horizontal, vertical;

    // Use this for initialization
    void Start () {
        rb2D = GetComponent<Rigidbody2D>();
        gameObject.layer = 9;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        movement = Vector2.zero;

        if (parent == null)
        {

        }
        else
        {
            Vector3 Delta = (parent.position - transform.position);
            float dist = Delta.magnitude;
            if (dist > 0.4f)
            {
                movement += (Vector2)Delta.normalized * (dist - 0.4f) * sens * Time.fixedDeltaTime;
            }
        }

        if (parent2 == null)
        {

        }
        else
        {
            Vector3 Delta = (parent2.position - transform.position);
            float dist = Delta.magnitude;
            if (dist > 0.4f)
            {
                movement += (Vector2)Delta.normalized * (dist - 0.4f) * sens * Time.fixedDeltaTime;
            }
        }

        // Movement
        if (horizontal != "" && vertical !="")
        {
            moveX = Input.GetAxisRaw(horizontal);
            moveY = Input.GetAxisRaw(vertical);
            movement += new Vector2(moveX, moveY) * Time.fixedDeltaTime * speed;
        }



        //rb2D.velocity = movement;
        if (movement != Vector2.zero)
        {
            rb2D.AddForce(movement);
        }
        else
        {
            rb2D.velocity = Vector2.zero;
        }
        

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 9)
        {
            Physics2D.IgnoreCollision(col.transform.GetComponent<CircleCollider2D>(), GetComponent<CircleCollider2D>());
        }
    }
}
