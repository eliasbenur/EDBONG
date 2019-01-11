using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope_Point : MonoBehaviour {

    public bool p_free;
    public Vector3 new_pos_p1;
    public Vector3 new_pos_p2;
    public Vector3 new_pos;

    public bool close_coll;

    // Use this for initialization
    void Start () {
        var collider = gameObject.AddComponent<CircleCollider2D>();
        collider.radius = 1f;
        //collider.isTrigger = true;

        Rigidbody2D rb2D = gameObject.AddComponent<Rigidbody2D>();
        rb2D.gravityScale = 0;
        rb2D.freezeRotation = true;
        rb2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb2D.mass = 1;

        close_coll = false;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
