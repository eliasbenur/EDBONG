using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Verlet_Movement : MonoBehaviour {

    public float speed, moveX, moveY;
    public Vector2 movement;
    public string horizontal, vertical;


    private void Start()
    {
    }

    void FixedUpdate()
    {
        moveX = (Input.GetAxisRaw(horizontal)) * speed * Time.fixedDeltaTime;
        moveY = (Input.GetAxisRaw(vertical))* speed * Time.fixedDeltaTime;
        if (gameObject.name == "PlayerOne")
        {
            //GameObject.Find("Rope_System").GetComponent<Verlet_Rope_System>().mov_P1 = new Vector2(-0.2f, 0);
            GameObject.Find("Rope_System").GetComponent<Verlet_Rope_System>().mov_P1 = new Vector2(moveX, moveY);
        }
        else if (gameObject.name == "PlayerTwo")
        {
            //GameObject.Find("Rope_System").GetComponent<Verlet_Rope_System>().mov_P2 = new Vector2(0.2f, 0);
            GameObject.Find("Rope_System").GetComponent<Verlet_Rope_System>().mov_P2 = new Vector2(moveX, moveY);
        }
    }
}
