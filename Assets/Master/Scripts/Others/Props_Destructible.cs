using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Props_Destructible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            GetComponent<Animator>().SetBool("destroy", true);
        }
    }
}
