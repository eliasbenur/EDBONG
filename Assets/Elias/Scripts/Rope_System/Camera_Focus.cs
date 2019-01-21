using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Focus : MonoBehaviour
{
    public List<Transform> targets;
    public Rope_System rope_system;

    public Vector3 offset;
    public float smoothTime = .5f;

    private Vector3 velocity;

    public Camera camera;

    public void Start()
    {
        //GetComponent<Camera>().orthographicSize = (1920 / (((1920 / 1080) * 2) * 48)) * 4;
        //targets.Clear();
        //targets.Add(rope_system.Points[0].transform);
        //targets.Add(rope_system.Points[rope_system.NumPoints - 1].transform);
    }

    private void Update()
    {
        update_cam();
    }

    public void update_cam()
    {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;
        camera.transform.position = Vector3.SmoothDamp(camera.transform.position, newPosition, ref velocity, smoothTime);
        //camera.transform.position = newPosition;
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
