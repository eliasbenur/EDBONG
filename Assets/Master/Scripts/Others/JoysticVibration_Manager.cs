using UnityEngine;
using XInputDotNetPure;
using Rewired;

public class JoysticVibration_Manager : MonoBehaviour
{
    #region Properties
    //Controller Vibration
    GamePadState prevState;
    public bool alreadyVibrated;
    public float leftMotor_RopeHit, rightMotor_RopeHit;
    public float leftMotor_EnnemyHit, rightMotor_EnnemyHit;
    public Player r_player;
    #endregion

    private void Start()
    {
        r_player = GetComponent<Player_Movement>().rew_player;
    }

    public void Vibrate_Control()
    {
        r_player.StopVibration();
        alreadyVibrated = false;
    }

    public void Vibrate_Control_Kill()
    {
        foreach (Joystick j in r_player.controllers.Joysticks)
        {
            if (!j.supportsVibration) continue;
            if (j.vibrationMotorCount > 0) j.SetVibration(0, leftMotor_RopeHit);
            if (j.vibrationMotorCount > 1) j.SetVibration(1, rightMotor_RopeHit);
        }
    }

    public void Vibrate_Control_Hit()
    {
        foreach (Joystick j in r_player.controllers.Joysticks)
        {
            if (!j.supportsVibration) continue;
            if (j.vibrationMotorCount > 0) j.SetVibration(0, leftMotor_EnnemyHit);
            if (j.vibrationMotorCount > 1) j.SetVibration(1, rightMotor_EnnemyHit);
        }
    }
}
