using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast_Triger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "obj_mov" || collision.tag == "enemy_mov" || collision.tag == "Objects")
        {
            //transform.parent.GetComponent<Rope_Point>().close_coll = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "obj_mov" || collision.tag == "enemy_mov" || collision.tag == "Objects")
        {
            transform.parent.GetComponent<Rope_Point>().close_coll = false;
        }
    }
}
