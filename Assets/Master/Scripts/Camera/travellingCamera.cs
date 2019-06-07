using UnityEngine;

public class travellingCamera : MonoBehaviour
{
    public CinematicBars cinematic;
    public GameObject canvas;

    public float smoothTime = 2f;
    private Vector3 velocity;

    public GameObject target;
    public float offsetCamera;

    private void FixedUpdate()
    {
        cinematic.Show(200, 0.8f);
        canvas.SetActive(false);
        transform.position = Vector3.SmoothDamp(Camera.main.transform.position, new Vector3(target.transform.position.x, target.transform.position.y - offsetCamera, transform.position.z), ref velocity, smoothTime);
    }
}
