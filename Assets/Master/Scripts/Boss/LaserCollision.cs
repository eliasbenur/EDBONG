using UnityEngine;

public class LaserCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "player")
        {
            if (collision.name == "PlayerOne")
            {
                if (!collision.gameObject.GetComponent<God_Mode>().godMode)
                {
                    collision.gameObject.GetComponent<God_Mode>().Hit_verification("PlayerOne", collision.transform.position, "Boss - Laser");
                }
            }
            else
            {
                if (!collision.gameObject.GetComponent<God_Mode>().godMode)
                {
                    collision.gameObject.GetComponent<God_Mode>().Hit_verification("PlayerTwo", collision.transform.position, "Boss - Laser");
                }
            }     
        }

    }
}
