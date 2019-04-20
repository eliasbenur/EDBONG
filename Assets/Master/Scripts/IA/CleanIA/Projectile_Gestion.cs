using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Gestion : MonoBehaviour
{
    public GameObject smoke;

    private void Awake()
    {
        smoke = (GameObject) Resources.Load("SmokeGameObject");
    }

    //If a projectile touch a wall then we make him disappear, but if it's a player we trigger the Hit fonction
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            if (collision.name == "PlayerOne")
            {
                if (!Camera.main.GetComponent<GameManager>().godMode_p1)
                {
                    Camera.main.GetComponent<GameManager>().Hit_p1();
                    Destroy(this.gameObject);
                }
            }
            else
            {
                if (!Camera.main.GetComponent<GameManager>().godMode_p2)
                {
                    Camera.main.GetComponent<GameManager>().Hit_p2();
                    Destroy(this.gameObject);
                }
            }
        }

        if (collision.gameObject.layer == 11)
        {
            Instantiate(smoke, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
