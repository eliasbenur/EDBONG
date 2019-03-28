using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triangle_Projectile : MonoBehaviour
{
    public GameObject targetObject;
    public float enemySpeed = 500;

    public float timer, timerTot;
    bool confirmed;

    private void Awake()
    {
        timerTot = 0.85f;
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
                    targetObject = GameObject.Find("PlayerOne");
                break;

                case 2:
                    targetObject = GameObject.Find("PlayerTwo");
                break;
            }
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Rigidbody2D>().AddForce((targetObject.transform.position - transform.position).normalized * enemySpeed);

            var rotationUpdate = targetObject.transform.position - transform.position;

            float rot_z = Mathf.Atan2(rotationUpdate.y, rotationUpdate.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            Camera.main.GetComponent<GameManager>().Hit();
            Destroy(this.gameObject);
        }

        if (collision.gameObject.layer == 19)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            Camera.main.GetComponent<GameManager>().Hit();
            Destroy(this.gameObject);
        }

        if (collision.gameObject.layer == 19)
        {
            Destroy(this.gameObject);
        }
    }
}
