using UnityEngine;

public class ButtonMashingAnimation : MonoBehaviour
{
    public float speed_scale;
    public float max_size, min_size;
    private float knownDistance;
    private float timeToGo;

    // Update is called once per frame
    void Update()
    {
        knownDistance = max_size - min_size;
        timeToGo = knownDistance / speed_scale;
        transform.localScale = new Vector3(Mathf.Lerp(min_size, max_size, Mathf.PingPong(Time.time*timeToGo, 1)), Mathf.Lerp(min_size,max_size,Mathf.PingPong(Time.time*timeToGo, 1)), 0);
    }
}
