using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trig_Rooms : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            if (gameObject.name == "Trig_Room1")
            {
                gameObject.GetComponentInParent<Proto_Gestion>().room_act = 1;
            }
            else if (gameObject.name == "Trig_Room2")
            {
                gameObject.GetComponentInParent<Proto_Gestion>().room_act = 2;
            }
            else if (gameObject.name == "Trig_Room3")
            {
                gameObject.GetComponentInParent<Proto_Gestion>().room_act = 3;
            }
            else if (gameObject.name == "Trig_Room4")
            {
                gameObject.GetComponentInParent<Proto_Gestion>().room_act = 4;
            }
            else if (gameObject.name == "Trig_Room5")
            {
                gameObject.GetComponentInParent<Proto_Gestion>().room_act = 5;
            }
        }
    }
}
