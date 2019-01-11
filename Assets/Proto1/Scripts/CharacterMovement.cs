using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {

    private float moveX, moveY;

    public string LeftRight;
    public string UpDown;

    public float speed;
    private Vector2 mouvement;
    public bool invertMovement;

    // Update is called once per frame
    void Update()
    {
        moveX = Input.GetAxisRaw(LeftRight);
        moveY = Input.GetAxisRaw(UpDown);

        RotateSprite();

        //With the rotation of the sprite, we have to invert commands to keep the same action 
        if (!invertMovement)
            Move(moveX, moveY);
        else
            Move(-moveX, moveY);

    }

    private void Move(float MoveX, float MoveY)
    {
        mouvement.Set(MoveX, MoveY);
        mouvement = mouvement.normalized * speed * Time.deltaTime;
        transform.Translate(mouvement);
    }

    //Rotate the sprite, depends where the player is moving
    private void RotateSprite()
    {
        if (moveX > 0)
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
            invertMovement = false;
        }
        else if (moveX < 0)
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
            invertMovement = true;
        }

    }
}
