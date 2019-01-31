using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class encer_trig2 : MonoBehaviour
{
    // Update is called once per frame
    public bool Check_isTouching()
    {
        //Check if colliders are touching Rope's Collider 
        LayerMask mask = LayerMask.GetMask("Rope");
        if (gameObject.GetComponent<CircleCollider2D>().IsTouchingLayers(mask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            //If collision is detected then the coll_state value change to true
            collision.gameObject.GetComponent<Rope_Point>().coll_state = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            //Else coll_state change to false
            collision.gameObject.GetComponent<Rope_Point>().coll_state = false;
        }
    }
}
