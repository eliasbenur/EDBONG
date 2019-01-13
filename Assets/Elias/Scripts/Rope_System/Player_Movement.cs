using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Movement : MonoBehaviour {

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

    public Animator animator;
    public float idle_anim_time;

    public bool rope_position;


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
        idle_anim_time = -1;
    }

    private void LateUpdate()
    {
        GameObject.Find("GameSystem").GetComponent<Camera_Focus>().update_cam();
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


        if (Input.GetKeyDown(KeyCode.E) && dash_v <= 0 && movement != Vector2.zero)
        {
            dash_v = dash_delay;
        }

        moveX = Input.GetAxisRaw(horizontal);
        moveY = Input.GetAxisRaw(vertical);

        if (moveX > 0)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if(moveX < 0)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }

        animator.SetInteger("input_x", Mathf.RoundToInt(moveX));
        animator.SetInteger("input_y", Mathf.RoundToInt(moveY));

        idle_anim();

        Move(moveX, moveY);

        //UI

        dash_bar.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position) + new Vector3(20,35,0);
        dash_bar.fillAmount = dash_v / dash_delay;

        LR.SetPosition(1, gameObject.transform.position);
        LR.SetPosition(0, gameObject.transform.position + (Vector3)movement.normalized * 20);
    }

    void idle_anim()
    {
        if (moveX == 0 && moveY == 0 && idle_anim_time == -1)
        {
            idle_anim_time = Random.Range(0, 10.0f);
            animator.SetBool("idle_right_bool", false);
        }else if (moveX != 0 || moveY != 0 )
        {
            idle_anim_time = -1;
        }

        if (idle_anim_time > 0)
        {
            idle_anim_time -= Time.fixedDeltaTime;
        }
        else if (idle_anim_time > -1 && idle_anim_time <= 0)
        {
            animator.SetBool("idle_right_bool", true);
            idle_anim_time = -1;
        }
    }

    void Move(float MoveX, float MoveY)
    {
        movement.Set(moveX, moveY);

        // OTHER ROPES

        if (rope_position)
        {
            if (dash_v > (dash_delay - dash_time))
            {
                movement = movement * dash_power;
            }
            //GameObject.Find("Rope_System").GetComponent<Rope_System>().mov_P1 = new Vector2(-1,0) * 3;
            GameObject.Find("Rope_System").GetComponent<Rope_System>().mov_P1 = movement;
        }
        else
        {
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

        //
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
