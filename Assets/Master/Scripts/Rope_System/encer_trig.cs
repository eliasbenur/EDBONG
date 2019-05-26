using UnityEngine;

public class encer_trig : MonoBehaviour
{
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


    /*If a point of chain collides, the point change to coll_state*/
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
