using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour {

    public GameObject OpenDoor1, OpenDoor2;
    public GameObject CloseDoor1, CloseDoor2;

    public int jenaimarre;

    private void Update()
    {
        if(jenaimarre == 2)
        {
            CloseDoor1.SetActive(false);
            CloseDoor2.SetActive(false);

            OpenDoor1.SetActive(true);
            OpenDoor2.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "player")
        {
            jenaimarre++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {

            jenaimarre--;
        }
    }
}
