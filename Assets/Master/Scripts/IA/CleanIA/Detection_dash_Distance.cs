using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection_dash_Distance : MonoBehaviour
{
    Initialisation_Rope checkPlayers;

    private void Awake()
    {
        checkPlayers = GetComponent<Initialisation_Rope>();
    }

    //Check whenever a player is dashing
    public bool Player_dashing()
    {
        if (checkPlayers.allPlayers.Count > 0)
        {
            if (checkPlayers.allPlayers[0].GetComponent<Player_Movement>().dash_v > (checkPlayers.allPlayers[0].GetComponent<Player_Movement>().dash_delay - checkPlayers.allPlayers[0].GetComponent<Player_Movement>().dash_time)
                || checkPlayers.allPlayers[1].GetComponent<Player_Movement>().dash_v > (checkPlayers.allPlayers[1].GetComponent<Player_Movement>().dash_delay - checkPlayers.allPlayers[1].GetComponent<Player_Movement>().dash_time))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    //Get the distance between the monster and the other object
    public float GetDistance(GameObject obj)
    {
        float distance = Vector2.Distance(obj.transform.position, transform.position);
        return distance;
    }
}
