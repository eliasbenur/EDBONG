using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Verlet_Rope_Point : MonoBehaviour {

    public bool p_free;
    public Vector3 OldPosition;
    public Vector3 NewPosition;
    public Vector3 StickPosition;

    private Transform _transform;

    // Use this for initialization
    void Start()
    {

        var collider = gameObject.AddComponent<CircleCollider2D>();
        collider.radius = 0.05f;
        collider.isTrigger = true;

        Rigidbody2D rb2D = gameObject.AddComponent<Rigidbody2D>();
        rb2D.gravityScale = 0;

    }

    public Vector3 position{
        get { return _transform.position; }
        set { _transform.position = value; }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
