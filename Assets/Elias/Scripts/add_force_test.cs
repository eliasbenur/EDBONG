using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class add_force_test : MonoBehaviour {

    Vector2 force;
    public GameObject objective;
    float distance;

	// Use this for initialization
	void Start () {
        force = new Vector2(1,0);
        distance = 3f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 AB = transform.position - objective.transform.position;
        if (AB.magnitude > distance)
        {
            //GetComponent<Rigidbody2D>().AddForce(AB.normalized * (distance - AB.magnitude), ForceMode2D.Impulse);
            GetComponent<Rigidbody2D>().velocity = AB.normalized * (distance - AB.magnitude);
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }
        
	}
}
