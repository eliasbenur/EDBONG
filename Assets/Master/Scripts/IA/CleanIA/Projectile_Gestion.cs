using System.Collections.Generic;
using UnityEngine;

public class Projectile_Gestion : MonoBehaviour
{
    #region Properties
    public GameObject smoke;
    private List<God_Mode> players;
    #endregion

    private void Awake()
    {
        players = Camera.main.GetComponent<GameManager>().players;
    }

    //If a projectile touch a wall then we make him disappear, but if it's a player we trigger the Hit fonction
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "PlayerOne")
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].name == collision.name && !players[i].godMode)
                    players[i].Hit_verification("PlayerOne", collision.transform.position, "Boss - Projectile Gestion");
            }
        }
        else
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].name == collision.name && !players[i].godMode)
                    players[i].Hit_verification("PlayerTwo", collision.transform.position, "Boss - Projectile Gestion");
            }
        }

        if (collision.gameObject.layer == 11)
        {
            Instantiate(smoke, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
