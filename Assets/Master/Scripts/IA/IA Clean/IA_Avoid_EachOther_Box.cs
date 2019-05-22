using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_Avoid_EachOther_Box : MonoBehaviour
{

    //Premier et Dernier point de la corde
    private GameObject rope_p_point;
    private GameObject rope_d_point;


    public void Start()
    {
        Transform ref_ = GameObject.Find("Rope_System").GetComponent<Rope_System>().transform;
        rope_p_point = ref_.GetChild(0).gameObject;
        rope_d_point = ref_.GetChild(ref_.childCount - 1).gameObject;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "rope")
        {
            if (collision.gameObject.name != rope_p_point.name && collision.gameObject.name != rope_d_point.name)
            {
                Physics2D.IgnoreCollision(collision.gameObject.GetComponent<CircleCollider2D>(), GetComponent<BoxCollider2D>(), true);
            }
        }
    }
}
