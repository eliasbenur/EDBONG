using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTeaser : MonoBehaviour
{
    public GameObject canvas;
    public CinematicBars cinematic;
    public float smoothTime = 2f;
    private Vector3 velocity;

    public GameObject target;
    public float offsetCamera;

    private void Start()
    {
        canvas.SetActive(false);
    }
    private void Update()
    {
        cinematic.Show(200, 0.8f);
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(Camera.main.transform.position, new Vector3(target.transform.position.x, target.transform.position.y - offsetCamera, transform.position.z), ref velocity, smoothTime);
    }
}
