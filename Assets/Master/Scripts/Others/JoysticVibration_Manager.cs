using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class JoysticVibration_Manager : MonoBehaviour
{
    //Controller Vibration
    public PlayerIndex playerIndex;
    GamePadState prevState;
    public bool alreadyVibrated;
    public float leftMotor_RopeHit, rightMotor_RopeHit;
    public float leftMotor_EnnemyHit, rightMotor_EnnemyHit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Vibrate_Control(float leftMotor, float rightMotor)
    {
        GamePad.SetVibration(playerIndex, leftMotor, rightMotor);
    }

    public void Vibrate_Control_Kill()
    {
        GamePad.SetVibration(playerIndex, leftMotor_RopeHit, rightMotor_RopeHit);
    }

    public void Vibrate_Control_Hit()
    {
        GamePad.SetVibration(playerIndex, leftMotor_EnnemyHit, rightMotor_EnnemyHit);
    }
}
