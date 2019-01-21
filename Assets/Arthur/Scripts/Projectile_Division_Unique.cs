using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Division_Unique : MonoBehaviour
{
    public bool division;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Boss")
            division = true;
    }
}
