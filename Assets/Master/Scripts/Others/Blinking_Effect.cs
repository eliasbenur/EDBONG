using UnityEngine;

public class Blinking_Effect : MonoBehaviour
{

    //Blincking Effect
    [HideInInspector] public float spriteBlinkingTimer = 0.0f;
    public float spriteBlinkingMiniDuration = 0.1f;
    [HideInInspector] public float spriteBlinkingTotalTimer;
    public float spriteBlinkingTotalDuration;

    private Player_Movement player_Movement;
    private JoysticVibration_Manager joysticVibration_Manager;

    [HideInInspector] public bool spawn = true;
    public Material default_sprite;
    public Material flash_sprite;


    // Start is called before the first frame update
    void Start()
    {
        player_Movement = GetComponent<Player_Movement>();
        joysticVibration_Manager = GetComponent<JoysticVibration_Manager>();
    }


    public void SpriteBlinkingEffect()
    {
        spriteBlinkingTotalTimer += Time.deltaTime;
        if (spriteBlinkingTotalTimer >= spriteBlinkingTotalDuration)
        {
            if (player_Movement != null)
            {
                player_Movement.startBlinking = false;
                joysticVibration_Manager.alreadyVibrated = true;
            }
            else
            {
                spawn = false;
            }
            spriteBlinkingTotalTimer = 0.0f;
            //this.gameObject.GetComponent<SpriteRenderer>().enabled = true;   // according to your sprite
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
            GetComponent<SpriteRenderer>().material = default_sprite;                 
            return;
        }

        spriteBlinkingTimer += Time.deltaTime;
        if (spriteBlinkingTimer >= spriteBlinkingMiniDuration)
        {
            spriteBlinkingTimer = 0.0f;
            if (this.gameObject.GetComponent<SpriteRenderer>().color == new Color(255, 255, 255, 150))
            {
                GetComponent<SpriteRenderer>().material = default_sprite;
                this.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);                
            }
            else
            {
                GetComponent<SpriteRenderer>().material = flash_sprite;
                this.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 150);               
            }
        }
    }
}
