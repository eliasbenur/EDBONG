using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWaveScript : MonoBehaviour
{
    public float speed;
    public float lengthMax;
    // Update is called once per frame
    void Update()
    {
        transform.localScale += new Vector3(speed, speed, speed);
        if(transform.localScale.x > lengthMax)
        {
            Destroy(this.gameObject);
        }
    }
}
