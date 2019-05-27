using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinking_Effect : MonoBehaviour
{

    //Blincking Effect
    public float spriteBlinkingTimer = 0.0f;
    public float spriteBlinkingMiniDuration = 0.1f;
    public float spriteBlinkingTotalTimer;
    public float spriteBlinkingTotalDuration;

    private Player_Movement player_Movement;
    private JoysticVibration_Manager joysticVibration_Manager;


    // Start is called before the first frame update
    void Start()
    {
        player_Movement = GetComponent<Player_Movement>();
        joysticVibration_Manager = GetComponent<JoysticVibration_Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SpriteBlinkingEffect()
    {
        spriteBlinkingTotalTimer += Time.deltaTime;
        if (spriteBlinkingTotalTimer >= spriteBlinkingTotalDuration)
        {
            player_Movement.startBlinking = false;
            joysticVibration_Manager.alreadyVibrated = true;
            spriteBlinkingTotalTimer = 0.0f;
            //this.gameObject.GetComponent<SpriteRenderer>().enabled = true;   // according to your sprite
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
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
}
