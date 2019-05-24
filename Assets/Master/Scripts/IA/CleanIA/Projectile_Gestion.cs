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
                    collision.gameObject.GetComponent<God_Mode>().Hit_verification("PlayerTwo", collision.transform.position, "Boss - Projectile Gestion");
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
