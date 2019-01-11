using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player2_Movement : MonoBehaviour {

    public float speed, moveX, moveY;
    public Vector2 movement;
    public string horizontal, vertical;

    //DASH
    public float dash_power;
    public float dash_time;
    public float dash_v;
    public float dash_delay;
    LineRenderer LR;
    public Image dash_bar;

    private Rigidbody2D rg2D;

    public bool auto_movement;


    private void Start()
    {
        rg2D = GetComponent<Rigidbody2D>();
        dash_v = 0;
        dash_time = 0.2f;
        LR = gameObject.GetComponent<LineRenderer>();
        LR.startWidth = 0.2f;
        LR.endWidth = 0.2f;
        LR.startColor = Color.gray;
        LR.endColor = Color.gray;
        LR.SetPosition(1, gameObject.transform.position);
        LR.SetPosition(0, gameObject.transform.position);
        Material whiteDiffuseMat = new Material(Shader.Find("Unlit/Texture"));
        LR.material = whiteDiffuseMat;
    }

    void FixedUpdate()
    {
        if (dash_v > 0)
        {
            dash_v -= Time.fixedDeltaTime;
        }
        else
        {
            dash_v = 0;
        }


        if (Input.GetKeyDown(KeyCode.I) && dash_v <= 0 && movement != Vector2.zero)
        {
            dash_v = dash_delay;
        }

        moveX = Input.GetAxisRaw(horizontal);
        moveY = Input.GetAxisRaw(vertical);

        Move(moveX, moveY);

        //UI

        dash_bar.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position) + new Vector3(20, 35, 0);
        dash_bar.fillAmount = dash_v / dash_delay;

        LR.SetPosition(1, gameObject.transform.position);
        LR.SetPosition(0, gameObject.transform.position + (Vector3)movement.normalized * 20);
    }

    void Move(float MoveX, float MoveY)
    {
        movement.Set(moveX, moveY);
        //AUTOMOVE
        if (auto_movement)
        {
            move_auto();
        }
        //
        movement = movement.normalized * speed  /* Time.fixedDeltaTime*/;
        if (dash_v > (dash_delay - dash_time))
        {
            movement = movement * dash_power;
        }
        transform.GetComponent<Rigidbody2D>().velocity = movement;

    }

    public void move_auto()
    {
        movement.Set(0, 1);

        if (transform.position.y > 14)
        {
            movement.Set(1, 0);
        }
    }
}
