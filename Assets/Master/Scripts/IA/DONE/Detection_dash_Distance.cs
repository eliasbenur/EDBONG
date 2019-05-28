using System.Collections.Generic;
using UnityEngine;

public class Detection_dash_Distance : MonoBehaviour
{
    #region Properties
    private Initialisation_Rope checkPlayers;
    [HideInInspector] public List<Player_Movement> players;
    #endregion 

    private void Awake()
    {
        checkPlayers = GetComponent<Initialisation_Rope>();
    }

    private void Start()
    {
        for (int i = 0; i < Camera.main.GetComponent<GameManager>().players_Movement.Count; i++)
            players.Add(Camera.main.GetComponent<GameManager>().players_Movement[i]);      
    }

    //Check whenever a player is dashing
    public bool Player_dashing()
    {
        if (checkPlayers.allPlayers.Count > 0)
            return (players[0].Dashing() || players[1].Dashing());
        else
            return false;
    }

    //Get the distance between the monster and the other object
    public float GetDistance(GameObject obj)
    {
        float distance = Vector2.Distance(obj.transform.position, transform.position);
        return distance;
    }
}
