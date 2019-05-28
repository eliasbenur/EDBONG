using UnityEngine;

public class encer_trig : MonoBehaviour
{
    [HideInInspector] public CircleCollider2D checkLayers_Touched;
    LayerMask mask;

    public void Awake()
    {
        mask = LayerMask.GetMask("Rope");
        checkLayers_Touched = gameObject.GetComponent<CircleCollider2D>();
    }

    public bool Check_isTouching()
    { 
        return (checkLayers_Touched.IsTouchingLayers(mask));
    }

    /*If a point of chain collides, the point change to coll_state*/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
            collision.gameObject.GetComponent<Rope_Point>().coll_state = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
            collision.gameObject.GetComponent<Rope_Point>().coll_state = false;
    }
}
