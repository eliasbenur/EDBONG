using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Space : MonoBehaviour {

    public float speed;
    Vector2 dir_speed;
    Vector2 accele;
    float gravite_planet;
    public Vector2 space_to_planet;
    public float dist_to_planet;

    // Use this for initialization
    void Start () {
        speed = 2;
        dir_speed = Vector3.zero;
        gravite_planet = 9.8f;
	}

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal") * speed;
        float moveY = Input.GetAxisRaw("Vertical") * speed;
        space_to_planet = new Vector2(moveX, moveY);
    }

    // Update is called once per frame
    void FixedUpdate () {

        //space_to_planet = (Vector2)(GameObject.Find("Planet").transform.position - transform.position);
        dist_to_planet = space_to_planet.magnitude;
        Vector2 acce_dir = space_to_planet.normalized;
        if (dist_to_planet!=0)
        {
            accele = (acce_dir * speed) / dist_to_planet * dist_to_planet;
        }
        else
        {
            accele = Vector2.zero;
        }
        Debug.Log(accele);
        transform.position += (Vector3)dir_speed * Time.fixedDeltaTime + ((Vector3)accele * (Time.fixedDeltaTime * Time.fixedDeltaTime))/2;
        dir_speed = dir_speed + accele * Time.fixedDeltaTime;
	}
}
