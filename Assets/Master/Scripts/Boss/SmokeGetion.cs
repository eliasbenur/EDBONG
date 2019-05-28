using UnityEngine;

public class SmokeGetion : MonoBehaviour
{
    public float timer, timerTot = 0.9f;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > timerTot)
            Destroy(gameObject);
    }
}
