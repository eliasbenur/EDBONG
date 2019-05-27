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
                if (!collision.gameObject.GetComponent<God_Mode>().godMode)
                {
                    collision.gameObject.GetComponent<God_Mode>().Hit_verification("PlayerOne", collision.transform.position, "Boss - Projectile Gestion");
                    Destroy(this.gameObject);
                }
            }
            else
            {
                if (!collision.gameObject.GetComponent<God_Mode>().godMode)
                {
                    collision.gameObject.GetComponent<God_Mode>().Hit_verification("PlayerTwo", collision.transform.position, "Boss - Projectile Gestion Phase");
                    Destroy(this.gameObject);
                }
            }
  
        }

        if (collision.gameObject.layer == 11)
        {
            Destroy(this.gameObject);
        }

        if(collision.gameObject.layer == 10)
        {
            Destroy(this.gameObject);
        }
    }
}
