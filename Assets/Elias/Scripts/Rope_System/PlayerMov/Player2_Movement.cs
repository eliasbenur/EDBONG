using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class Player2_Movement : MonoBehaviour {

    public float speed, moveX, moveY;
    public Vector2 movement;
    public string horizontal, vertical;
    public string horizontal_clavier, vertical_clavier;
    public bool clavier_active;

    //DASH
    public float dash_power;
    public float dash_time;
    public float dash_v;
    public float dash_delay;
    public Image dash_bar;

    public bool auto_movement;

    public Animator animator;
    public float idle_anim_time;

    public bool rope_position;

    public Rope_System rope_system;

    //Blincking Effect
    public float spriteBlinkingTimer = 0.0f;
    public float spriteBlinkingMiniDuration = 0.1f;
    public float spriteBlinkingTotalTimer = 0.0f;
    public float spriteBlinkingTotalDuration = 1.0f;
    public bool startBlinking = false;

    //Camera Shake Effect
    // Transform of the GameObject you want to shake
    public Transform cameraTransform;
    public float shakeDuration;
    public float shakeMagnitude;
    public float dampingSpeed;
    public Vector3 initialPosition;

    GameManager checkLifePlayers;

    //Controller Vibration
    public PlayerIndex playerIndex;
    GamePadState prevState;
    public bool testVibrationHitRope;
    public float timerRope;

    public bool alreadyVibrated;

    public float timerTot_RopeHitVibrate;
    public float leftMotor_RopeHit, rightMotor_RopeHit;
    public float leftMotor_EnnemyHit, rightMotor_EnnemyHit;

    public float shakeDuration_RopeHit;
    public float shakeMagnitude_RopeHit;
    public float dampingSpeed_RopeHit;


    [SerializeField]
    public GameManager CheckMoney;

    public Collider2D collisionItems;

    private void Awake()
    {
        cameraTransform = Camera.main.GetComponent<Transform>();
        checkLifePlayers = Camera.main.GetComponent<GameManager>();
        CheckMoney = Camera.main.GetComponent<GameManager>();
        collisionItems = GetComponent<Collider2D>();
    }

    private void Start()
    {
        dash_v = 0;
        dash_time = 0.2f;
        idle_anim_time = -1;
    }

    private void Update()
    {
        if (testVibrationHitRope)
        {
            Vibrate_Control(leftMotor_RopeHit, rightMotor_RopeHit);
            initialPosition = Camera.main.transform.position;
            CameraShake_RopeHit();
            timerRope += Time.deltaTime;
            if (timerRope > timerTot_RopeHitVibrate)
            {
                testVibrationHitRope = false;
                timerRope = 0;
            }
        }

        if (!testVibrationHitRope || alreadyVibrated)
            Vibrate_Control(0, 0);

        if (startBlinking)
        {
            alreadyVibrated = false;
            Vibrate_Control(leftMotor_EnnemyHit, rightMotor_EnnemyHit);
            initialPosition = Camera.main.transform.position;
            shakeDuration = 0.3f;
            SpriteBlinkingEffect();
            CameraShake();
            checkLifePlayers.godMode = true;
        }

        transform.position = rope_system.Points[rope_system.NumPoints - 1].transform.position;
        //gameObject.GetComponent<Rigidbody2D>().MovePosition(rope_system.Points[rope_system.NumPoints - 1].transform.position);
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


        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Joystick2Button0)) && dash_v <= 0 && movement != Vector2.zero)
        {
            dash_v = dash_delay;
            CheckMoney.timerGodMode = 1.5f;
            CheckMoney.godMode = true;
        }

        if (clavier_active)
        {
            moveX = Input.GetAxisRaw(horizontal_clavier);
            moveY = Input.GetAxisRaw(vertical_clavier);
        }
        else
        {
            moveX = Input.GetAxisRaw(horizontal);
            moveY = Input.GetAxisRaw(vertical);
        }

        if (moveX > 0)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (moveX < 0)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }

        if (moveX > 0)
        {
            animator.SetInteger("input_x", 1);
        }
        else if (moveX < 0)
        {
            animator.SetInteger("input_x", -1);
        }
        else
        {
            animator.SetInteger("input_x", 0);
        }

        if (moveY > 0)
        {
            animator.SetInteger("input_y", 1);
        }
        else if (moveY < 0)
        {
            animator.SetInteger("input_y", -1);
        }
        else
        {
            animator.SetInteger("input_y", 0);
        }

        //animator.SetInteger("input_x", Mathf.RoundToInt(moveX));
        //animator.SetInteger("input_y", Mathf.RoundToInt(moveY));

        idle_anim();

        Move(moveX, moveY);

        //UI

        dash_bar.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position) + new Vector3(20, 35, 0);
        dash_bar.fillAmount = dash_v / dash_delay;
    }

    void idle_anim()
    {
        if (moveX == 0 && moveY == 0 && idle_anim_time == -1)
        {
            idle_anim_time = Random.Range(10.0f, 30.0f);
            animator.SetBool("idle_right_bool", false);
        }
        else if (moveX != 0 || moveY != 0)
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
        movement.Normalize();

        // OTHER ROPES

        if (rope_position)
        {
            if (dash_v > (dash_delay - dash_time))
            {
                movement = movement * dash_power;
                animator.SetBool("dash", true);
            }
            else
            {
                animator.SetBool("dash", false);
            }
            GameObject.Find("Rope_System").GetComponent<Rope_System>().mov_P2 = movement * speed;
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Item")
        {
            CheckMoney.KeyPressed = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Trap")
        {
            //checkLifePlayers.Hit();
        }

        if (collision.tag == "Coin")
        {
            CheckMoney.money++;
            Destroy(collision.gameObject);
        }

        if (collision.tag == "Item")
        {
            CheckMoney.KeyPressed = true;
            collisionItems = collision;
        }

    }

    private void SpriteBlinkingEffect()
    {
        spriteBlinkingTotalTimer += Time.deltaTime;
        if (spriteBlinkingTotalTimer >= spriteBlinkingTotalDuration)
        {
            startBlinking = false;
            alreadyVibrated = true;
            spriteBlinkingTotalTimer = 0.0f;
            this.gameObject.GetComponent<SpriteRenderer>().enabled = true;   // according to your sprite
            return;
        }

        spriteBlinkingTimer += Time.deltaTime;
        if (spriteBlinkingTimer >= spriteBlinkingMiniDuration)
        {
            spriteBlinkingTimer = 0.0f;
            if (this.gameObject.GetComponent<SpriteRenderer>().enabled == true)
            {
                this.gameObject.GetComponent<SpriteRenderer>().enabled = false;  //make changes
            }
            else
            {
                this.gameObject.GetComponent<SpriteRenderer>().enabled = true;   //make changes
            }
        }
    }

    void CameraShake()
    {
        if (shakeDuration > 0)
        {
            cameraTransform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            cameraTransform.localPosition = initialPosition;
            checkLifePlayers.godMode = false;
        }
    }

    void CameraShake_RopeHit()
    {
        if (shakeDuration_RopeHit > 0)
        {
            cameraTransform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude_RopeHit;
            shakeDuration_RopeHit -= Time.deltaTime * dampingSpeed_RopeHit;
        }
        else
        {
            cameraTransform.localPosition = initialPosition;
            checkLifePlayers.godMode = false;
        }
    }



    public void Vibrate_Control(float leftMotor, float rightMotor)
    {
        GamePad.SetVibration(playerIndex, leftMotor, rightMotor);
    }
}
