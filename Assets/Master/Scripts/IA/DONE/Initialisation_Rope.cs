using System.Collections.Generic;
using UnityEngine;

public class Initialisation_Rope : MonoBehaviour
{
    #region Properties
    //List of all players on the scene
    [HideInInspector] public List<GameObject> allPlayers = new List<GameObject>();
    //Actual target of the monster
    [HideInInspector] public GameObject target;
    [HideInInspector] public Rope_System rope_system;
    #endregion

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
