using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialisation_Rope : MonoBehaviour
{
    //List of all players on the scene
    public List<GameObject> allPlayers = new List<GameObject>();
    //Actual target of the monster
    public GameObject target;
    public Rope_System rope_system;

    // Use this for initialization
    void Start()
    {
        //We find the Rope System, the target will be the center of the cain
        if (rope_system == null)
        {
            rope_system = GameObject.Find("Rope_System").GetComponent<Rope_System>();
        }
    }
}
