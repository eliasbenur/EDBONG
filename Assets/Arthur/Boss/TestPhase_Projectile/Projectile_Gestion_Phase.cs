using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Gestion_Phase : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            if (collision.name == "PlayerOne")
            {
                if (!Camera.main.GetComponent<GameManager>().godMode_p1)
                {
                    Camera.main.GetComponent<GameManager>().Hit_verification("PlayerOne", collision.transform.position, "Boss - Projectile Gestion Phase");
                    Destroy(this.gameObject);
                }
            }
            else
            {
                if (!Camera.main.GetComponent<GameManager>().godMode_p2)
                {
                    Camera.main.GetComponent<GameManager>().Hit_verification("PlayerTwo", collision.transform.position, "Boss - Projectile Gestion Phase");
                    Destroy(this.gameObject);
                }
            }
  
        }

        if (collision.gameObject.layer == 19)
        {
            Destroy(this.gameObject);
        }

        if (collision.gameObject.tag == "test")
        {
            Destroy(this.gameObject);
        }
    }
}
