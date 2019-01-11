using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceEnnemy : MonoBehaviour {

    private float life;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Rope")
        {
            Debug.Log("Collision with rope detected");
            if (life > 0)
                life--;
            else
            {
                Debug.Log("Ennemy sliced");
                Destroy(this.gameObject);
            }
                
        }
    } 
}
