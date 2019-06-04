using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_Static : MonoBehaviour
{
    private Player_Movement player_one;
    private Player_Movement player_two;

    private void Start()
    {
        player_one = GameObject.Find("PlayerOne").GetComponent<Player_Movement>();
        player_two = GameObject.Find("PlayerTwo").GetComponent<Player_Movement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            if (collision.name == "PlayerOne" && !player_one.Dashing())
            {
                player_one.gameObject.GetComponent<God_Mode>().Hit_verification("PlayerOne", player_one.transform.position, "Laser Static");
            }
            else if(collision.name == "PlayerTwo" && !player_two.Dashing())
            {
                player_two.gameObject.GetComponent<God_Mode>().Hit_verification("PlayerTwo", player_two.transform.position, "Laser Static");
            }
        }
    }
}
