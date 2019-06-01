using UnityEngine;
using UnityEngine.UI;

public class SliderDecreaseValue : MonoBehaviour
{
    public float speedDecrease;
    private Slider sliderValue;
    public float factorLevel;

    private void Awake()
    {
        sliderValue = GetComponent<Slider>();
    }
    // Update is called once per frame
    void Update()
    {
        speedDecrease = sliderValue.value * factorLevel;
        sliderValue.value -= speedDecrease * Time.deltaTime;
    }
}
