using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class encer_trig : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag != "player" && collision.transform.tag != "monster" && collision.transform.tag != "encer_trig_right" && collision.transform.tag != "encer_trig_up" && collision.transform.tag != "encer_trig_down" && collision.transform.tag != "encer_trig_left")
        {
            if (collision.transform.parent.tag == "rope")
            {
                switch (transform.tag)
                {
                    case "encer_trig_left":
                        transform.parent.GetComponent<basicAI2_E>().trig_left = true;
                        break;
                    case "encer_trig_right":
                        transform.parent.GetComponent<basicAI2_E>().trig_right = true;
                        break;
                    case "encer_trig_up":
                        transform.parent.GetComponent<basicAI2_E>().trig_up = true;
                        break;
                    case "encer_trig_down":
                        transform.parent.GetComponent<basicAI2_E>().trig_down = true;
                        break;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (transform.tag)
        {
            case "encer_trig_left":
                transform.parent.GetComponent<basicAI2_E>().trig_left = false;
                break;
            case "encer_trig_right":
                transform.parent.GetComponent<basicAI2_E>().trig_right = false;
                break;
            case "encer_trig_up":
                transform.parent.GetComponent<basicAI2_E>().trig_up = false;
                break;
            case "encer_trig_down":
                transform.parent.GetComponent<basicAI2_E>().trig_down = false;
                break;
        }
    }
}
