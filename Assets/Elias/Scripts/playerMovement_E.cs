using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement_E : MonoBehaviour {

    public float speed, moveX, moveY;
    public Vector2 movement;
    public string horizontal, vertical;
    public int player_num;

    private Rigidbody2D rg2D;

    private void Start()
    {
        rg2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate () {
        //moveX = Input.GetAxisRaw(horizontal);
        //moveY = Input.GetAxisRaw(vertical);
        moveX = -1;
        moveY = 0;
        Move(moveX, moveY);
	}

    void Move(float MoveX, float MoveY)
    {
        movement.Set(moveX, moveY);
        movement = movement.normalized * speed * Time.deltaTime;

        
        if (player_num == 1 && movement != Vector2.zero)
        {
            GameObject.Find("ParticleSystem_E").GetComponent<UParticleSystem_E>().Apply_Force_Ply1(movement.magnitude);
            transform.position += new Vector3(movement.x, movement.y, 0);
        }
        else if (player_num == 2)
        {
            //GameObject.Find("ParticleSystem_E").GetComponent<UParticleSystem_E>().PreformSubstep3(UseSubstep, Gravity);
        }




    }

}
