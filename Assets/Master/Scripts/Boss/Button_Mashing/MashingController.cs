using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MashingController : MonoBehaviour
{
    public List<Player_Movement> control;
    public float valueToAdd_Input;
    private Slider sliderValue;
    private SliderDecreaseValue decreaseStop;
    public float threesholdValue;
    public Image fill;
    public bool confirmed;

    private void Awake()
    {
        sliderValue = GetComponent<Slider>();
        decreaseStop = GetComponent<SliderDecreaseValue>();
        control = Camera.main.GetComponent<GameManager>().players_Movement;
    }

    // Update is called once per frame
    void Update()
    {
        if (sliderValue.value < sliderValue.maxValue - threesholdValue)
        {
            for(int i=0;i<control.Count;i++)
            {
                control[i].GetComponent<JoysticVibration_Manager>().Vibration_Control(sliderValue.value, sliderValue.value);
            }
            for (int i = 0; i < control.Count; i++)
            {
                if (control[i].rew_player.GetButtonDown("Dash"))
                {
                    sliderValue.value += valueToAdd_Input;
                }
            }
        }
        else
        {
            sliderValue.value = sliderValue.maxValue;
            Debug.Log("Kill Confirmed");
            confirmed = true;
            decreaseStop.enabled = false;
            fill.color = Color.green;
        }
    }
}
