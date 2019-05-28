using System.Collections.Generic;
using UnityEngine;

public class triangle_Projectile : MonoBehaviour
{
    #region Properties
    [HideInInspector] public GameObject targetObject;
    public float enemySpeed = 300;

    public float timerTot;
    private float timer;
    bool confirmed;
    private GameObject p1, p2;
    private Rigidbody2D rb_projectile;
    private List<God_Mode> players;
    #endregion

    private void Awake()
    {
        p1 = GameObject.Find("PlayerOne");
        p2 = GameObject.Find("PlayerTwo");
        rb_projectile.GetComponent<Rigidbody2D>();
        players = Camera.main.GetComponent<GameManager>().players;
    }
    

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > timerTot && !confirmed)
        {
            confirmed = true;
            int target = Random.Range(1, 3);
            switch (target)
            {
                case 1:
                    targetObject = p1;
                break;

                case 2:
                    targetObject = p2;
                break;
            }
            rb_projectile.velocity = Vector2.zero;
            rb_projectile.AddForce((targetObject.transform.position - transform.position).normalized * enemySpeed);

            var rotationUpdate = targetObject.transform.position - transform.position;

            float rot_z = Mathf.Atan2(rotationUpdate.y, rotationUpdate.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "PlayerOne")
        {
            for(int i =0; i < players.Count; i++)
            {
                if (players[i].name == collision.gameObject.name && !players[i].godMode)
                    players[i].Hit_verification("PlayerOne", collision.transform.position, "triangle_Projectile");

            }
        }
        else
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].name == collision.gameObject.name && !players[i].godMode)
                    players[i].Hit_verification("PlayerTwo", collision.transform.position, "triangle_Projectile");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            collision.gameObject.GetComponent<God_Mode>().Hit_verification("PlayerUndefined", collision.transform.position, "triangle_Projectile");
            Destroy(this.gameObject);
        }

        if (collision.gameObject.layer == 11)
        {
            Destroy(this.gameObject);
        }
    }
}
