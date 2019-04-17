using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End_AutoRun : MonoBehaviour
{
    private int num_player_inside;

    private void Stop_Runing()
    {
        transform.parent.GetComponent<Door_Trigger>().Stop_AutoRuning();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            num_player_inside++;
        }

        if (num_player_inside == 2)
        {
            Stop_Runing();
        }
    }
}
