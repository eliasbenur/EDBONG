using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_EnemiesColl : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            GameObject.Find("Group_EnemiesColl").GetComponent<Group_Enemiescoll_Trigger>().enabled = true;
            Destroy(gameObject);
        }
    }
}
