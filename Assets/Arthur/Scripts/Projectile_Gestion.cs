using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Gestion : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            Camera.main.GetComponent<GameManager>().Hit();
            Destroy(this.gameObject);
        }

        if (collision.gameObject.layer == 11)
        {
            Destroy(this.gameObject);
        }
    }
}
