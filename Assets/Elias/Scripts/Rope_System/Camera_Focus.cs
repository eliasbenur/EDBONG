using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Focus : MonoBehaviour
{
    public List<Transform> targets;

    public Vector3 offset;
    public float smoothTime = .5f;

    private Vector3 velocity;

    public Camera camera;

    public void Start()
    {
        //GetComponent<Camera>().orthographicSize = (1920 / (((1920 / 1080) * 2) * 48)) * 4;
    }

    public void update_cam()
    {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;
        if ((camera.transform.position - newPosition).magnitude > 10)
        {
            camera.transform.position = Vector3.SmoothDamp(camera.transform.position, newPosition, ref velocity, smoothTime);
        }

    }


    Vector3 GetCenterPoint()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }
}
