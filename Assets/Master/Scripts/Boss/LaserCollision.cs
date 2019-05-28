using System.Collections.Generic;
using UnityEngine;

public class LaserCollision : MonoBehaviour
{
    #region Properties
    private List<God_Mode> players;
    #endregion

    private void Awake()
    {
        players = Camera.main.GetComponent<GameManager>().players;    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "PlayerOne")
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].name == collision.name && !players[i].godMode)
                    players[i].Hit_verification("PlayerOne", collision.transform.position, "Boss - Laser");
            }
        }
        else
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].name == collision.name && !players[i].godMode)
                    players[i].Hit_verification("PlayerTwo", collision.transform.position, "Boss - Laser");
            }
        }
    }
}
