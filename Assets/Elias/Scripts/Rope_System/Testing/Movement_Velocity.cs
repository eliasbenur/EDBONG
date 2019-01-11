using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Velocity : MonoBehaviour {

    public float speed, moveX, moveY;
    public Vector2 movement;
    public string horizontal, vertical;

    private Rigidbody2D rg2D;


    private void Start()
    {
        rg2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        moveX = Input.GetAxisRaw(horizontal);
        moveY = Input.GetAxisRaw(vertical);
        movement = new Vector2(moveX, moveY) * Time.fixedDeltaTime * speed;

        GetComponent<Rigidbody2D>().velocity += movement;
    }
}
