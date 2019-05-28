using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;
using Rewired;

public class Player_Movement : MonoBehaviour {

    public float speed;
    private float movementX, movementY;
    private Vector2 movement;
    private bool can_move = true;
    private bool auto_movement;

    public Rope_System rope_system;
    private God_Mode god_ModeAction;
    private JoysticVibration_Manager joysticVibrationMan;
    private Camere_Shake_Manager camera_ShakeMan;
    private Blinking_Effect blinking_Effect;

    //DASH
    public float dash_power;
    // the time that the dash takes
    public float dash_time;
    // delay between 2 dashes
    public float dash_delay;
    private float dash_tmp = 0;
    //Direction of the Dash
    private Vector2 dash_direction;
    public Image dash_bar;

    //Players Inputs
    public enum Enum_PlayerNum {PlayerOne = 1, PlayerTwo = 2};
    public bool modo_solo;
    public Enum_PlayerNum PlayerNum;
    public Player rew_player;

    //Animation
    private Animator animator;
    private float idle_anim_time;

    //Vibrations + Shake + Blinkning
    public bool testVibrationHitRope;
    public float timerRope;
    public float timerTot_RopeHitVibrate;
    public bool startBlinking = false;



    private void Awake()
    {
        god_ModeAction = GetComponent<God_Mode>();
        animator = GetComponent<Animator>();
        joysticVibrationMan = GetComponent<JoysticVibration_Manager>();
        camera_ShakeMan = Camera.main.GetComponent<Camere_Shake_Manager>();
        blinking_Effect = GetComponent<Blinking_Effect>();
    }

    public bool Dashing()
    {
        if (dash_tmp > (dash_delay - dash_time))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Start()
    {
        idle_anim_time = -1;

        //Player Inputs
        set_Solo_Mode();
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
        if (god_ModeAction.godMode == false)
            god_ModeAction.timerTotGodMode = god_ModeAction.oldValueTimerGod;


        ///////// A CLEAN ////////////////
        if (testVibrationHitRope)
        {
            joysticVibrationMan.Vibrate_Control_Kill();
            camera_ShakeMan.start_Shake_Kill(0.2f);
            timerRope += Time.deltaTime;
            if (timerRope > timerTot_RopeHitVibrate)
            {
                testVibrationHitRope = false;
                god_ModeAction.godMode = false;
                timerRope = 0;  
            }
        }

        if (!testVibrationHitRope || joysticVibrationMan.alreadyVibrated)
            joysticVibrationMan.Vibrate_Control(0, 0);

        if (startBlinking)
        {
            joysticVibrationMan.alreadyVibrated = false;
            joysticVibrationMan.Vibrate_Control_Hit();
            camera_ShakeMan.start_Shake_Hit(0.3f);
            blinking_Effect.SpriteBlinkingEffect();
        }
        /////////////////////////

        if (PlayerNum == Enum_PlayerNum.PlayerOne && rope_system.get_points().Count > 0)
        {
            transform.position = rope_system.get_points()[0].transform.position;
        }
        else if(PlayerNum == Enum_PlayerNum.PlayerTwo && rope_system.get_points().Count > 0)
        {
            transform.position = rope_system.get_points()[rope_system.NumPoints - 1].transform.position;
        }


        if (dash_tmp > 0)
        {
            dash_tmp -= Time.fixedDeltaTime;
        }
        else
        {
            dash_tmp = 0;
        }

        if (modo_solo)
        {
            if (PlayerNum == Enum_PlayerNum.PlayerOne)
            {
                if (can_move)
                {
                    movementX = rew_player.GetAxis("MoveHorizontal_p1");
                    movementY = rew_player.GetAxis("MoveVertical_p1");
                }
                if (rew_player.GetButtonDown("Dash_p1") && dash_tmp <= 0 && new Vector2(movementX,movementY) != Vector2.zero)
                {
                    dash_tmp = dash_delay;
                    dash_direction = new Vector2(movementX,movementY).normalized;
                    //rope_system.transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = false;
                    Physics2D.IgnoreLayerCollision(19, 21);
                    god_ModeAction.timerGodMode = 1.5f;
                    god_ModeAction.godMode = true;
                    SoundManager.PlaySound(SoundManager.Sound.PlayerDash);
                }
            }
            else if (PlayerNum == Enum_PlayerNum.PlayerTwo)
            {
                if (can_move)
                {
                    movementX = rew_player.GetAxis("MoveHorizontal_p2");
                    movementY = rew_player.GetAxis("MoveVertical_p2");
                }

                if (rew_player.GetButtonDown("Dash_p2") && dash_tmp <= 0 && new Vector2(movementX, movementY) != Vector2.zero)
                {
                    dash_tmp = dash_delay;
                    dash_direction = new Vector2(movementX, movementY).normalized;
                    // rope_system.transform.GetChild(rope_system.transform.childCount - 1).GetComponent<CircleCollider2D>().enabled = false;
                    Physics2D.IgnoreLayerCollision(20, 21);
                    god_ModeAction.timerGodMode = 1.5f;
                    god_ModeAction.godMode = true;
                    SoundManager.PlaySound(SoundManager.Sound.PlayerDash);
                }
            }
        }
        else
        {
            if (can_move)
            {
                movementX = rew_player.GetAxis("MoveHorizontal");
                movementY = rew_player.GetAxis("MoveVertical");
            }

            if (rew_player.GetButtonDown("Dash") && dash_tmp <= 0 && new Vector2(movementX, movementY) != Vector2.zero)
            {
                dash_tmp = dash_delay;
                dash_direction = new Vector2(movementX, movementY).normalized;

                if (PlayerNum == Enum_PlayerNum.PlayerOne)
                {
                    //rope_system.transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = false;
                    Physics2D.IgnoreLayerCollision(19, 21);
                    SoundManager.PlaySound(SoundManager.Sound.PlayerDash);

                    god_ModeAction.timerGodMode = 1.5f;
                    god_ModeAction.godMode = true;
                }
                else if (PlayerNum == Enum_PlayerNum.PlayerTwo)
                {
                    //rope_system.transform.GetChild(rope_system.transform.childCount - 1).GetComponent<CircleCollider2D>().enabled = false;
                    Physics2D.IgnoreLayerCollision(20, 21);
                    SoundManager.PlaySound(SoundManager.Sound.PlayerDash);

                    god_ModeAction.timerGodMode = 1.5f;
                    god_ModeAction.godMode = true;
                }             
            }
        }

        if (movementX > 0)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (movementX < 0)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }

        //Runing Animation
        if (movementX == 0 && movementY == 0)
        {
            animator.SetInteger("input_x", 0);
            animator.SetInteger("input_y", 0);
        }
        else if (Mathf.Abs(movementX) > Mathf.Abs(movementY))
        {
            if (movementX > 0)
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
            if (movementY > 0)
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

        Move(movementX, movementY);

        //Audio
        if (movementX != 0 || movementY != 0)
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
        dash_bar.fillAmount = dash_tmp / dash_delay;



    }

    void idle_anim()
    {
        if (movementX == 0 && movementY == 0 && idle_anim_time == -1)
        {
            idle_anim_time = Random.Range(10.0f, 30.0f);
            animator.SetBool("idle_right_bool", false);
        }else if (movementX != 0 || movementY != 0 )
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
        movement.Set(movementX, movementY);

        bool hole_coll = false;

        //int layerMask = 1 << 21; 
        int layerMask = 32768; // 15
        int layerMask2 = 2097152; //21
        int layermasr_f = layerMask | layerMask2;

        if (!(dash_tmp > (dash_delay - dash_time)))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, movement.normalized, movement.magnitude, layermasr_f); // 2 ^ 21
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
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, movement.normalized, movement.magnitude, 32768); // 2 ^ 21
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

        if (dash_tmp > (dash_delay - dash_time))
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
                rope_system.set_mov_P1(movement * dash_power);
                god_ModeAction.timerTotGodMode = 0.2f;
                god_ModeAction.godMode = true;
            }
            else if (PlayerNum == Enum_PlayerNum.PlayerTwo)
            {
                rope_system.set_mov_P2(movement * dash_power);
                god_ModeAction.timerTotGodMode = 0.2f;
                god_ModeAction.godMode = true;
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
                rope_system.set_mov_P1(movement * speed);
            }
            else if (PlayerNum == Enum_PlayerNum.PlayerTwo)
            {
                rope_system.set_mov_P2(movement * speed);
            }
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
            if (dash_tmp > (dash_delay - dash_time))
            {
                //Physics2D.IgnoreCollision(collision.gameObject.GetComponent<CircleCollider2D>(), GetComponent<BoxCollider2D>(), true);
            }
        }
    }


    public void Stop_Moving()
    {
        can_move = false;
        movementX = 0;
        movementY = 0;
    }

    public void Allow_Moving()
    {
        can_move = true;
    }

    public float get_MovementX()
    {
        return movementX;
    }

    public float get_MovementY()
    {
        return movementY;
    }

    public void set_MovementX(float movement_x)
    {
        movementX = movement_x;
    }

    public void set_MovementY(float movement_y)
    {
        movementY = movement_y;
    }

    public bool Is_Moving()
    {
        if (movementX != 0 || movementY != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }




    private Vector2 PixelPerfectClamp(Vector2 moveVector, float pixelsPerUnit)
    {
        Vector2 vectorInPixels = new Vector2(
            Mathf.RoundToInt(moveVector.x * pixelsPerUnit),
            Mathf.RoundToInt(moveVector.y * pixelsPerUnit));

        return vectorInPixels / pixelsPerUnit;
    }
}
