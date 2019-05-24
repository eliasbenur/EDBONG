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
                Camera.main.GetComponent<God_Mode>().Hit_verification("PlayerOne", collision.transform.position, "Boss - Laser");
            }
            else
            {
                Camera.main.GetComponent<God_Mode>().Hit_verification("PlayerTwo", collision.transform.position, "Boss - Laser");
            }
        }

    }
}
