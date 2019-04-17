using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Division_Unique : MonoBehaviour
{
    public bool division;

    private void OnTriggerExit2D(Collider2D collision)
    {
        //The boss has a circle trigger all around him, when the projectile exit it, then he can be divide
        if (collision.tag == "Boss")
            division = true;
    }
}
