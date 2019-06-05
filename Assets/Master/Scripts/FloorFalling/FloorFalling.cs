using UnityEngine;

public class FloorFalling : MonoBehaviour
{
    private Animator floor;
    public Collider2D colliderDestroy;

    private void Awake()
    {
        floor = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            if(!floor.enabled && colliderDestroy !=null)
            {
                floor.enabled = true;
                colliderDestroy.enabled = false;
            }
        }           
    }
    
}
