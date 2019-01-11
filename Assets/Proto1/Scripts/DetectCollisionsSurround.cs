using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollisionsSurround : MonoBehaviour {

    public List<String> CollidedObjects = new List<String>();
    public List<GameObject> RopeObjects = new List<GameObject>();
    private string colliderOn;
    private string colliderOff;

    public float percentage;

    private void Awake()
    {
        foreach (GameObject fooObj in GameObject.FindGameObjectsWithTag("Rope"))
        {
            RopeObjects.Add(fooObj);
        }
    }

    void Update()
    {
        //When an ennemy is surround, check how many colliders are colliding compared to all objects with the tag "Rope"
        //If we have more than 75 % of them we can start to decrease the ennemy's life
        if (CollidedObjects != null)
        {
            percentage = CollidedObjects.Count;
            percentage = (percentage / RopeObjects.Count) * 100;
            if (percentage > 75f)
                DmgTriggersOnCollisionDetect();
        }
        else
            percentage = 0;
    }

    private void DmgTriggersOnCollisionDetect()
    {
        Debug.Log("Damage are triggers, enough colliders required");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Each time he detect a new collider with the rope's tag, he add him in his list
        if(collision.gameObject.tag == "Rope")
        {
            colliderOn = collision.gameObject.name;
            if (CollidedObjects.Contains(colliderOn))
            {
                Debug.Log("This collider has already been detected");
            }
            else
            {
                CollidedObjects.Add(colliderOn);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Find which element in list exit and kick him off the list
        if(collision.gameObject.tag =="Rope")
        {
            colliderOff = collision.gameObject.name;
            if (CollidedObjects.Contains(colliderOff))
            {
                CollidedObjects.Remove(colliderOff);
            }
            else
            {
                Debug.Log("Error, this collider wasn't in the list, what happened ?");
                return;
            }
        }
    }

}
