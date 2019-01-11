using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_Point_Test : MonoBehaviour {

    public GameObject Point1;
    public float dist;
    public float desired_dist;
    public CircleCollider2D collid;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 Delta = Point1.transform.position - gameObject.transform.position;
        dist = Delta.magnitude;

        Collider2D[] coll2D = new Collider2D[10];
        ContactFilter2D contf = new ContactFilter2D();
        contf.NoFilter();

        /*if (dist > desired_dist)
        {
            transform.position += Delta.normalized * (dist - desired_dist);*/
        // if (collid.bounds.Contains((Vector2)transform.position))
        if (collid.OverlapCollider(contf, coll2D) > 0)
        {
                ColliderDistance2D coll_distance = gameObject.GetComponent<CircleCollider2D>().Distance(collid);
                Debug.DrawLine(transform.position, transform.position + (Vector3)coll_distance.normal * coll_distance.distance, Color.green);
                transform.position += (Vector3)coll_distance.normal * coll_distance.distance;
            }

           /* Delta = Point1.transform.position - gameObject.transform.position;
            dist = Delta.magnitude;
            transform.position += Delta.normalized * (dist - desired_dist);
        }*/
	}
}
