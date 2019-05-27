using System.Collections.Generic;
using UnityEngine;

public class Camera_Focus : MonoBehaviour{
    
    #region Properties
    //Public Var
    public Vector3 offset;
    public float smoothTime = .5f;

    //Refs to GameObjects
    private new Camera camera;
    private List<Transform> targets = new List<Transform>();

    // Velocity that is moving the camera with SmoothDamp
    private Vector3 velocity;  
    #endregion


    public void Awake(){
        camera = GetComponent<Camera>();
        Transform ply = GameObject.Find("PlayerOne").transform;
        targets.Add(ply);
        ply = GameObject.Find("PlayerTwo").transform;   
        targets.Add(ply);
    }

    private void Update(){
        Update_cam();
    }

    /* Updates the position of the Camera, with SmoothDamp */
    public void Update_cam(){
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;
        camera.transform.position = Vector3.SmoothDamp(camera.transform.position, newPosition, ref velocity, smoothTime);
    }

    /* Get the center point between the 2 targets */
    Vector3 GetCenterPoint(){
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.center;
    }

    public List<Transform> GetCameraTargets(){
        return targets;
    }
}
