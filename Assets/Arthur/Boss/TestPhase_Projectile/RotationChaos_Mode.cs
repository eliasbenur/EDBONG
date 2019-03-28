using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationChaos_Mode : MonoBehaviour
{
    public float speed;
    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.parent.transform.position, Vector3.back, speed * Time.deltaTime);
    }
}
