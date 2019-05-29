using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camere_Shake_Manager : MonoBehaviour
{
    public Vector3 initialPosition;
    public float shakeDuration_RopeHit;
    public Transform cameraTransform;
    public float shakeMagnitude_RopeHit;
    public float dampingSpeed_RopeHit;
    public God_Mode god_Mode;
    public float shakeDuration;
    public float shakeMagnitude;
    public float dampingSpeed;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.GetComponent<Transform>();
        god_Mode = GetComponent<God_Mode>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void start_Shake_Kill(float duration)
    {
        initialPosition = Camera.main.transform.position;
        CameraShake_RopeHit();
        shakeDuration_RopeHit = duration;
    }

    public void start_Shake_Hit(float duration)
    {
        initialPosition = Camera.main.transform.position;
        shakeDuration = duration;
        CameraShake();
    }


    void CameraShake_RopeHit()
    {
        if (shakeDuration_RopeHit > 0)
        {
            cameraTransform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude_RopeHit;
            shakeDuration_RopeHit -= Time.deltaTime * dampingSpeed_RopeHit;
        }
        else
        {
            cameraTransform.localPosition = initialPosition;
        }
    }


    public void CameraShake()
    {
        if (shakeDuration > 0)
        {
            cameraTransform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            cameraTransform.localPosition = initialPosition;
        }
    }
}
