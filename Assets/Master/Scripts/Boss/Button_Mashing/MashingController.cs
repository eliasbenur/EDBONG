using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MashingController : MonoBehaviour
{
    public List<Player_Movement> control;
    public float valueToAdd_Input;
    private Image sliderValue;
    private SliderDecreaseValue decreaseStop;
    public float threesholdValue;
    public Image fill;
    public bool confirmed;

    private void Awake()
    {
        sliderValue = GameObject.Find("Smashing_Fill").GetComponent<Image>();
        decreaseStop = GetComponent<SliderDecreaseValue>();
        control = Camera.main.GetComponent<GameManager>().players_Movement;
    }

    // Update is called once per frame
    void Update()
    {
        if (sliderValue.fillAmount < 1 - threesholdValue)
        {
            for(int i=0;i<control.Count;i++)
            {
                control[i].GetComponent<JoysticVibration_Manager>().Vibration_Control(sliderValue.fillAmount, sliderValue.fillAmount);
            }
            for (int i = 0; i < control.Count; i++)
            {
                if (control[i].rew_player.GetButtonDown("Dash"))
                {
                    sliderValue.fillAmount += valueToAdd_Input;
                }
            }
        }
        else
        {
            sliderValue.fillAmount = 1;
            confirmed = true;
            decreaseStop.enabled = false;
            fill.color = Color.green;
        }
    }
}
