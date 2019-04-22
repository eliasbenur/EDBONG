using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCollision : MonoBehaviour
{
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "player")
        {
            if (collision.name == "PlayerOne")
            {
                Camera.main.GetComponent<GameManager>().Hit_p1();
            }
            else
            {
                Camera.main.GetComponent<GameManager>().Hit_p2();
            }
        }

    }
}
