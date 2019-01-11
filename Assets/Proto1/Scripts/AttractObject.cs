using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractObject : MonoBehaviour {

    private List<GameObject> allPlayers = new List<GameObject>();
    private float oldSpeed;

    private void Awake()
    {
        foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("tack"))
        {
            allPlayers.Add(Obj);
        }
        oldSpeed = allPlayers[0].GetComponent<CharacterMovement>().speed;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag =="Rope")
        {
            foreach(GameObject objet in allPlayers)
            {
                objet.GetComponent<CharacterMovement>().speed = 1;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag =="Rope")
        {
            foreach(GameObject objet in allPlayers)
            {
                objet.GetComponent<CharacterMovement>().speed = oldSpeed;
            }
        }
    }
}
