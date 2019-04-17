using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleShot : MonoBehaviour
{
    public float speed_Rotation_Projectile;

    // Update is called once per frame
    void Update()
    {
        //Script for a monster who have gameobject all around him, we make them rotate around 
        //It allow us to do some "tentacle" of projectile 
        transform.RotateAround(transform.parent.position, new Vector3(0,0,1), speed_Rotation_Projectile * Time.deltaTime);
        transform.rotation = Quaternion.identity;
    }
}
