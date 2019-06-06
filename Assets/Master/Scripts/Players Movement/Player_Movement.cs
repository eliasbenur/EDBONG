using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class Player_Movement : MonoBehaviour {

    public float speed;
    public float y_offset;
    private float movementX, movementY;
    private Vector2 movement;
    private bool can_move = true;
    private bool auto_movement;
    private bool hole_coll;

    public Rope_System rope_system;
    private God_Mode god_ModeAction;
    private JoysticVibration_Manager joysticVibrationMan;
    private Camere_Shake_Manager camera_ShakeMan;
    private Blinking_Effect blinking_Effect;

    //DASH
    public float dash_power = 30;
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

    public Menu_Manager pause;
    private GameManager manager;
    public Animator miniMap;

    public  float timer;
    public float timerMiniMap = 1f;

    private void Awake()
    {
        god_ModeAction = GetComponent<God_Mode>();
        animator = GetComponent<Animator>();
        joysticVibrationMan = GetComponent<JoysticVibration_Manager>();
        camera_ShakeMan = Camera.main.GetComponent<Camere_Shake_Manager>();
        blinking_Effect = GetComponent<Blinking_Effect>();
        pause = Camera.main.GetComponent<Menu_Manager>();
        manager = Camera.main.GetComponent<GameManager>();
    }

    public bool Dashing()
    {
        return (dash_tmp > (dash_delay - dash_time));
    }

    private void Start()
    {
        idle_anim_time = -1;

        //Player Inputs
        set_Solo_Mode();
    }

    public void Start_Dash()
    {      
        dash_tmp = dash_delay;
        dash_direction = new Vector2(movementX, movementY).normalized;
        // God Mode ini
        god_ModeAction.timerGodMode = 1.5f;
        god_ModeAction.godMode = true;
       
        AkSoundEngine.PostEvent("play_dash", Camera.main.gameObject);
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
        if (miniMap.GetBool("Open"))
            timer += Time.deltaTime;

        if (miniMap.GetBool("Open") && miniMap.GetBool("Close") )
        {
            timer = 0;
        }

        //Reset God Mode timer
        if (god_ModeAction.godMode == false)
        god_ModeAction.timerTotGodMode = god_ModeAction.oldValueTimerGod;

        //UI control with the controller
        if (manager.life <= 0)
            pause.cheatModeButton.SetActive(false);
        /*
        if (ReInput.controllers.joystickCount > 0 && !pause.cheatMode.activeSelf && !modo_solo)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }*/

        if (rew_player.GetButtonDown("Pause") && !modo_solo)
        {
            if (manager.life > 0)
            {
                if (pause.menu.activeSelf)
                {
                    pause.cheatModeButton.SetActive(true);
                    pause.menu.SetActive(false);
                    Time.timeScale = 1;
                }
                else
                {
                    pause.cheatModeButton.SetActive(false);
                    pause.menu.SetActive(true);
                    Time.timeScale = 0;
                }
            }
        }

        if (rew_player.GetButtonDown("CheatMode"))
        {        
            if (!miniMap.GetBool("Open"))
                miniMap.SetBool("Open", true);

            if(!miniMap.GetBool("Close") && timer >= timerMiniMap)
            {
                timer = 0;
                miniMap.SetBool("Close", true);
            }
        }

        ///////// HIT SYSTEM - A CLEAN ////////////////
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
            joysticVibrationMan.Vibrate_Control();

        if (startBlinking)
        {
            joysticVibrationMan.alreadyVibrated = false;
            joysticVibrationMan.Vibrate_Control_Hit();
            camera_ShakeMan.start_Shake_Hit(0.3f);
            blinking_Effect.SpriteBlinkingEffect();
        }
        /////////////////////////

        //Set the position of the Sprite to the extrems of the chains
        if (PlayerNum == Enum_PlayerNum.PlayerOne && rope_system.get_points().Count > 0)
        {
            transform.position = rope_system.get_points()[0].transform.position + new Vector3(0,- y_offset,0); ;
        }
        else if(PlayerNum == Enum_PlayerNum.PlayerTwo && rope_system.get_points().Count > 0)
        {
            transform.position = rope_system.get_points()[rope_system.NumPoints - 1].transform.position + new Vector3(0, -y_offset, 0); ; ;
        }

        // Dash timer
        if (dash_tmp > 0)
        {
            dash_tmp -= Time.deltaTime;
        }
        else
        {
            dash_tmp = 0;
        }

        //Geting the direction of the movement and detecting if the player started a dash
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
                    Start_Dash();
                    Physics2D.IgnoreLayerCollision(19, 21);
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
                    Start_Dash();
                    //To cros the "Holes"
                    Physics2D.IgnoreLayerCollision(20, 21);
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
                Debug.Log("DASH!");
                Start_Dash();
                if (PlayerNum == Enum_PlayerNum.PlayerOne)
                {
                    Physics2D.IgnoreLayerCollision(19, 21);
                }
                else if (PlayerNum == Enum_PlayerNum.PlayerTwo)
                {
                    Physics2D.IgnoreLayerCollision(20, 21);
                }
                
            }
        }

        //Sprite Fliping
        if (movementX > 0)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (movementX < 0)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }

        Set_inputs_Animation();

        // If Player dont move, play idle anim
        idle_anim();

        //UI Dash
        dash_bar.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position) + new Vector3(20, 35, 0);
        dash_bar.fillAmount = dash_tmp / dash_delay;

        Move(movementX, movementY);
    }

    /* Setting what animation is going to play, depending of the inputs of the players*/
    void Set_inputs_Animation()
    {
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
            }
            else
            {
                animator.SetInteger("input_x", -1);
            }
            animator.SetInteger("input_y", 0);

        }
        else
        {
            if (movementY > 0)
            {
                animator.SetInteger("input_y", 1);
            }
            else
            {
                animator.SetInteger("input_y", -1);
            }
            animator.SetInteger("input_x", 0);
        }
    }

    /*If the player stop moving, we start a idle timer that will play a speacil idel animation */
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
            idle_anim_time -= Time.deltaTime;
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

        check_MovementLimits();


        if (Dashing())
        {
            if (!hole_coll)
            {
                movement = dash_direction;
            }

            animator.SetBool("dash", true);
            god_ModeAction.timerTotGodMode = 0.2f;
            god_ModeAction.godMode = true;

            if (PlayerNum == Enum_PlayerNum.PlayerOne)
            {
                rope_system.set_mov_P1(movement * dash_power);
            }
            else if (PlayerNum == Enum_PlayerNum.PlayerTwo)
            {
                rope_system.set_mov_P2(movement * dash_power);
            }
        }
        else
        {
            animator.SetBool("dash", false);

            if (PlayerNum == Enum_PlayerNum.PlayerOne)
            {
                rope_system.set_mov_P1(movement * speed);
                //Reactive the collision betwen the Holes and the player when is not dashing
                if (Physics2D.GetIgnoreLayerCollision(19, 21))
                {
                    Physics2D.IgnoreLayerCollision(19, 21, false);
                }
            }
            else if (PlayerNum == Enum_PlayerNum.PlayerTwo)
            {
                rope_system.set_mov_P2(movement * speed);
                //Reactive the collision betwen the Holes and the player when is not dashing
                if (Physics2D.GetIgnoreLayerCollision(20, 21))
                {
                    Physics2D.IgnoreLayerCollision(20, 21, false);
                }
            }
        }
    }

    public void check_MovementLimits()
    {
        //int layerMask = 1 << 21; 
        int layerMask = 32768; // 15 - Door
        int layerMask2 = 2097152; //21 - Coll_Hole
        int layermasr_f = layerMask | layerMask2;

        hole_coll = false;

        /* blocks the player movement versus the doors ( and the holes if they are not dashing) to prevent that the players cros the colliders*/
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
    }


    public void Stop_Moving()
    {
        can_move = false;
        movementX = 0;
        movementY = 0;
        dash_tmp = 0;
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

}
