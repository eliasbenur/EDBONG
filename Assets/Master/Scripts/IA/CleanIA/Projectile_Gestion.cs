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
            Camera.main.GetComponent<GameManager>().Hit();
            Destroy(this.gameObject);
        }

        if (collision.gameObject.layer == 11)
        {
            Instantiate(smoke, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
