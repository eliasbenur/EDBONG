using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour {

    public float speed, moveX, moveY;
    public Vector2 movement;
    public string horizontal, vertical;

	// Update is called once per frame
	void Update () {
        moveX = Input.GetAxisRaw(horizontal);
        moveY = Input.GetAxisRaw(vertical);
        Move(moveX, moveY);
	}

    void Move(float MoveX, float MoveY)
    {
        movement.Set(moveX, moveY);
        movement = movement.normalized * speed * Time.deltaTime;
        transform.Translate(movement);
    }
}
