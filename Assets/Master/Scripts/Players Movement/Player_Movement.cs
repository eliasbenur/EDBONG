using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;
using Rewired;
using UnityEngine.Analytics;

public class Player_Movement : MonoBehaviour {

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
    private Vector2 dash_direction;
    public Image dash_bar;

    //Players Inputs
    public enum Enum_PlayerNum {PlayerOne = 1, PlayerTwo = 2};
    public bool modo_solo;
    public Enum_PlayerNum PlayerNum;
    public Player rew_player;

    public bool auto_movement;

    public Animator animator;
    public float idle_anim_time;

    public Rope_System rope_system;

    public AudioSource running_audio;

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


    public GameManager camera_GameManager;

    public Collider2D collisionItems;

    public bool can_move = true;

    private void Awake()
    {
        cameraTransform = Camera.main.GetComponent<Transform>();
        checkLifePlayers = Camera.main.GetComponent<GameManager>();
        camera_GameManager = Camera.main.GetComponent<GameManager>();
        collisionItems = GetComponent<Collider2D>();
    }

    private void Start()
    {
        dash_v = 0;
        idle_anim_time = -1;

        //Player Inputs
        set_Solo_Mode();

        /*AnalyticsResult result = Analytics.CustomEvent("Test");
        // This should print "Ok" if the event was sent correctly.
        Debug.Log(result);

        AnalyticsEvent.LevelStart(1);

        AnalyticsEvent.Custom("Analytics Event");
        Analytics.CustomEvent("Analytics X", transform.position);*/

        Physics2D.IgnoreLayerCollision(8, 18);


    }

    public void set_Solo_Mode()
    {
        if (modo_solo)
        {
            rew_player = ReInput.players.GetPlayer("PlayerOne");
        }
        else
        {
            if (PlayerNum == Enum_PlayerNum.PlayerOne)
            {
                rew_player = ReInput.players.GetPlayer("PlayerOne");
            }
            else
            {
                rew_player = ReInput.players.GetPlayer("PlayerTwo");
            }
        }
    }

    private void Update()
    {

        if (camera_GameManager.godMode_p1 == false)
            camera_GameManager.timerTotGodMode_p1 = camera_GameManager.oldValueTimerGod;
        if(camera_GameManager.godMode_p2 == false)
            camera_GameManager.timerTotGodMode_p2 = camera_GameManager.oldValueTimerGod;

        if (testVibrationHitRope)
        {
            Vibrate_Control(leftMotor_RopeHit, rightMotor_RopeHit);
            initialPosition = Camera.main.transform.position;
            CameraShake_RopeHit();
            shakeDuration_RopeHit = 0.2f;
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
            checkLifePlayers.godMode_p1 = true;
        }
        if (PlayerNum == Enum_PlayerNum.PlayerOne && rope_system.Points.Count > 0)
        {
            transform.position = rope_system.Points[0].transform.position;
        }
        else if(PlayerNum == Enum_PlayerNum.PlayerTwo && rope_system.Points.Count > 0)
        {
            transform.position = rope_system.Points[rope_system.NumPoints - 1].transform.position;
        }


        if (dash_v > 0)
        {
            dash_v -= Time.fixedDeltaTime;
        }
        else
        {
            dash_v = 0;
        }

        if (modo_solo)
        {
            if (PlayerNum == Enum_PlayerNum.PlayerOne)
            {
                if (can_move)
                {
                    moveX = rew_player.GetAxis("MoveHorizontal_p1");
                    moveY = rew_player.GetAxis("MoveVertical_p1");
                }
                if (rew_player.GetButtonDown("Dash_p1") && dash_v <= 0 && new Vector2(moveX,moveY) != Vector2.zero)
                {
                    dash_v = dash_delay;
                    dash_direction = new Vector2(moveX,moveY).normalized;
                    //rope_system.transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = false;
                    Physics2D.IgnoreLayerCollision(19, 21);
                    camera_GameManager.timerGodMode_p1 = 1.5f;
                    camera_GameManager.godMode_p1 = true;
                }
            }
            else if (PlayerNum == Enum_PlayerNum.PlayerTwo)
            {
                if (can_move)
                {
                    moveX = rew_player.GetAxis("MoveHorizontal_p2");
                    moveY = rew_player.GetAxis("MoveVertical_p2");
                }

                if (rew_player.GetButtonDown("Dash_p2") && dash_v <= 0 && new Vector2(moveX, moveY) != Vector2.zero)
                {
                    dash_v = dash_delay;
                    dash_direction = new Vector2(moveX, moveY).normalized;
                    // rope_system.transform.GetChild(rope_system.transform.childCount - 1).GetComponent<CircleCollider2D>().enabled = false;
                    Physics2D.IgnoreLayerCollision(20, 21);
                    camera_GameManager.timerGodMode_p2 = 1.5f;
                    camera_GameManager.godMode_p2 = true;
                }
            }
        }
        else
        {
            if (can_move)
            {
                moveX = rew_player.GetAxis("MoveHorizontal");
                moveY = rew_player.GetAxis("MoveVertical");
            }

            if (rew_player.GetButtonDown("Dash") && dash_v <= 0 && new Vector2(moveX, moveY) != Vector2.zero)
            {
                dash_v = dash_delay;
                dash_direction = new Vector2(moveX, moveY).normalized;

                if (PlayerNum == Enum_PlayerNum.PlayerOne)
                {
                    //rope_system.transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = false;
                    Physics2D.IgnoreLayerCollision(19, 21);
                }
                else if (PlayerNum == Enum_PlayerNum.PlayerTwo)
                {
                    //rope_system.transform.GetChild(rope_system.transform.childCount - 1).GetComponent<CircleCollider2D>().enabled = false;
                    Physics2D.IgnoreLayerCollision(20, 21);
                }

                camera_GameManager.timerGodMode_p1 = 1.5f;
                camera_GameManager.godMode_p1 = true;
            }
        }

        if (moveX > 0)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (moveX < 0)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }

        //Runing Animation
        if (moveX == 0 && moveY == 0)
        {
            animator.SetInteger("input_x", 0);
            animator.SetInteger("input_y", 0);
        }
        else if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
        {
            if (moveX > 0)
            {
                animator.SetInteger("input_x", 1);
                animator.SetInteger("input_y", 0);
            }
            else
            {
                animator.SetInteger("input_x", -1);
                animator.SetInteger("input_y", 0);
            }

        }else
        {
            if (moveY > 0)
            {
                animator.SetInteger("input_x", 0);
                animator.SetInteger("input_y", 1);
            }
            else
            {
                animator.SetInteger("input_x", 0);
                animator.SetInteger("input_y", -1);
            }
        }

        // If Player dont move, play idle anim
        idle_anim();

        Move(moveX, moveY);

        //Audio
        if (moveX != 0 || moveY != 0)
        {
            //running_audio.UnPause();
            //SoundManager.PlaySound(SoundManager.Sound.PlayerFTS);
        }
        else
        {
            //running_audio.Pause();
        }

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

    public void Move(float MoveX, float MoveY)
    {
        movement.Set(moveX, moveY);

        bool hole_coll = false;

        //int layerMask = 1 << 21; 
        if (!(dash_v > (dash_delay - dash_time)))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, movement.normalized, movement.magnitude, 2097152); // 2 ^ 21
            if (hit.collider != null)
            {
                movement = movement.normalized * hit.distance;
                hole_coll = true;
            }
            else
            {
                movement.Normalize();
            }
        }




        // OTHER ROPES

        if (dash_v > (dash_delay - dash_time))
        {
            ///////////////////

            ///////////////////
            ///
            if (!hole_coll)
            {
                movement = dash_direction;
            }

            animator.SetBool("dash", true);

            if (PlayerNum == Enum_PlayerNum.PlayerOne)
            {
                rope_system.mov_P1 = movement * dash_power;
                camera_GameManager.timerTotGodMode_p1 = 0.2f;
                camera_GameManager.godMode_p1 = true;
            }
            else if (PlayerNum == Enum_PlayerNum.PlayerTwo)
            {
                rope_system.mov_P2 = movement * dash_power;
                camera_GameManager.timerTotGodMode_p2 = 0.2f;
                camera_GameManager.godMode_p2 = true;
            }
        }
        else
        {
            animator.SetBool("dash", false);

            if (PlayerNum == Enum_PlayerNum.PlayerOne && Physics2D.GetIgnoreLayerCollision(19,21))
            {
                Physics2D.IgnoreLayerCollision(19, 21, false);

            }
            else if (PlayerNum == Enum_PlayerNum.PlayerTwo && Physics2D.GetIgnoreLayerCollision(20, 21))
            {
                Physics2D.IgnoreLayerCollision(20, 21, false);
            }

            if (PlayerNum == Enum_PlayerNum.PlayerOne)
            {
                rope_system.mov_P1 = movement * speed;
            }
            else if (PlayerNum == Enum_PlayerNum.PlayerTwo)
            {
                rope_system.mov_P2 = movement * speed;
            }
        }


    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Item")
        {
            camera_GameManager.KeyPressed = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
        if (collision.gameObject.tag == "coll_hole")
        {
            if (dash_v > (dash_delay - dash_time))
            {
                //Physics2D.IgnoreCollision(collision.gameObject.GetComponent<CircleCollider2D>(), GetComponent<BoxCollider2D>(), true);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Trap")
        {
            //checkLifePlayers.Hit();
        }

        if (collision.tag == "Coin")
        {
            camera_GameManager.money++;
            Destroy(collision.gameObject);
        }

        if (collision.tag == "Item")
        {
            camera_GameManager.KeyPressed = true;
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
            //this.gameObject.GetComponent<SpriteRenderer>().enabled = true;   // according to your sprite
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(255,255,255,255);
            return;
        }

        spriteBlinkingTimer += Time.deltaTime;
        if (spriteBlinkingTimer >= spriteBlinkingMiniDuration)
        {
            spriteBlinkingTimer = 0.0f;
            if (this.gameObject.GetComponent<SpriteRenderer>().color == new Color(255, 255, 255, 255))
            {
                this.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
            }
            else
            {
                this.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
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
            checkLifePlayers.godMode_p1 = false;
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
            checkLifePlayers.godMode_p1 = false;
        }
    }

    public void Stop_Moving()
    {
        can_move = false;
        moveX = 0;
        moveY = 0;
    }



    public void Vibrate_Control(float leftMotor, float rightMotor)
    {
        GamePad.SetVibration(playerIndex, leftMotor, rightMotor);
    }

    private Vector2 PixelPerfectClamp(Vector2 moveVector, float pixelsPerUnit)
    {
        Vector2 vectorInPixels = new Vector2(
            Mathf.RoundToInt(moveVector.x * pixelsPerUnit),
            Mathf.RoundToInt(moveVector.y * pixelsPerUnit));

        return vectorInPixels / pixelsPerUnit;
    }
}
