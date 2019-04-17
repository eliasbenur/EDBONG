using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class encer_trig : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public bool Check_isTouching()
    {
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
            collision.gameObject.GetComponent<Rope_Point>().coll_state = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            collision.gameObject.GetComponent<Rope_Point>().coll_state = false;
        }
    }


}
