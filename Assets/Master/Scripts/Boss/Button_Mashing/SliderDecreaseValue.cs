using UnityEngine;
using UnityEngine.UI;

public class SliderDecreaseValue : MonoBehaviour
{
    public float speedDecrease;
    private Image sliderValue;
    public float factorLevel;

    private void Awake()
    {
        sliderValue = GameObject.Find("Smashing_Fill").GetComponent<Image>();
    }
    // Update is called once per frame
    void Update()
    {
        speedDecrease = sliderValue.fillAmount * factorLevel;
        sliderValue.fillAmount -= speedDecrease * Time.deltaTime;
    }
}
