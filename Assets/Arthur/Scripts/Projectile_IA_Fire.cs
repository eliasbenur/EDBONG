using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_IA_Fire : MonoBehaviour
{
    GameObject target;
    public float ennemySpeed;

    private void Awake()
    {
        target = transform.parent.GetComponent<IA_Fire_Target>().target;
    }

    void FixedUpdate()
    {
        var direction = (target.transform.position - transform.position);
        direction = direction.normalized * ennemySpeed * Time.fixedDeltaTime;
        transform.Translate(direction);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "player")
        {
            if (collision.name == "PlayerOne")
            {
                if (!Camera.main.GetComponent<GameManager>().godMode_p1)
                {
                    Camera.main.GetComponent<GameManager>().Hit_p1();
                    Destroy(this.gameObject);
                }
            }
            else
            {
                if (!Camera.main.GetComponent<GameManager>().godMode_p2)
                {
                    Camera.main.GetComponent<GameManager>().Hit_p2();
                    Destroy(this.gameObject);
                }
            }
        }

    }
}
