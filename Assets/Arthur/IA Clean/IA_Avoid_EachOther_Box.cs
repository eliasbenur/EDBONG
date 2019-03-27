using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_Avoid_EachOther_Box : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "rope")
        {
            //Test corde non elastique
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<CircleCollider2D>(), GetComponent<BoxCollider2D>(), true);
            //Physics2D.IgnoreCollision(collision.gameObject.GetComponent<BoxCollider2D>(), GetComponent<CircleCollider2D>(), true);
        }
    }
}
