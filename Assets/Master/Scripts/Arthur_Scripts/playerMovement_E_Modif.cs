using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class playerMovement_E_Modif : MonoBehaviour {

    public float speed, moveX, moveY;
    public Vector2 movement;
    public string horizontal, vertical;

    private Rigidbody2D rg2D;

    [SerializeField]
    public GameManager CheckMoney;

    public Collider2D collisionItems;


    private void Start()
    {
        rg2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate () {
        moveX = Input.GetAxisRaw(horizontal);
        moveY = Input.GetAxisRaw(vertical);
        Move(moveX, moveY);
	}


    void Move(float MoveX, float MoveY)
    {
        movement.Set(moveX, moveY);
        //movement = movement.normalized * speed * Time.deltaTime;
        //transform.Translate(movement);
        rg2D.velocity = movement * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Coin")
        {
            CheckMoney.money++;
            Destroy(collision.gameObject);
        }

        if(collision.tag == "Item")
        {
            collisionItems = collision;
            CheckMoney.KeyPressed = true;         
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Item")
        {
            CheckMoney.KeyPressed = false;
        }
    }
}
