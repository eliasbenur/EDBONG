using Kilt.EasyRopes2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Movement : MonoBehaviour {

    public float speed;
    int direction;

    public Rope2D r2d;

	// Use this for initialization
	void Start () {
        direction = -1;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        /*if (transform.position.y < 9)
        {
            direction = 1;
        }
        else if(transform.position.y > 14)
        {
            direction = -1;
        }*/

        //transform.position += new Vector3(0, direction * speed * Time.fixedDeltaTime, 0);
        transform.GetComponent<Rigidbody2D>().velocity = new Vector3(0, direction * speed, 0);
        //transform.GetComponent<Rigidbody2D>().MovePosition(transform.position + new Vector3(0, direction * speed * Time.fixedDeltaTime, 0));

        //transform.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        //transform.GetComponent<Rigidbody2D>().AddForce(new Vector3(0, direction * speed * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);

        //transform.Translate(new Vector3(0, direction * speed * Time.fixedDeltaTime, 0));

    }

    
}
