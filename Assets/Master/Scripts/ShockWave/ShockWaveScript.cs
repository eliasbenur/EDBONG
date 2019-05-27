using UnityEngine;

public class ShockWaveScript : MonoBehaviour
{
    public float speed;
    public float lengthMax;


    void Update()
    {
        transform.localScale += new Vector3(speed, speed, speed);
        if(transform.localScale.x > lengthMax)
        {
            Destroy(this.gameObject);
        }
    }
}
