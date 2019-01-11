using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDetect : MonoBehaviour {
    private Transform objectTransfom;

    private float noMovementThreshold = 0.0001f;
    private const int noMovementFrames = 3;
    Vector3[] previousLocations = new Vector3[noMovementFrames];
    public bool isMoving;
    public UParticleSystem GetSumLength;

    //Let other scripts see if the object is moving
    public bool IsMoving
    {
        get { return isMoving; }
    }

    void Awake()
    {
        GetSumLength = GetComponent<UParticleSystem>();
        //For good measure, set the previous locations
        for (int i = 0; i < previousLocations.Length; i++)
        {
            previousLocations[i] = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update () {
        if (GetSumLength.somme > 2)
        {
            for (int i = 0; i < previousLocations.Length - 1; i++)
            {
                previousLocations[i] = previousLocations[i + 1];
            }
            previousLocations[previousLocations.Length - 1] = objectTransfom.position;

            //Check the distances between the points in your previous locations
            //If for the past several updates, there are no movements smaller than the threshold,
            //you can most likely assume that the object is not moving
            for (int i = 0; i < previousLocations.Length - 1; i++)
            {
                if (Vector3.Distance(previousLocations[i], previousLocations[i + 1]) >= noMovementThreshold)
                {
                    //The minimum movement has been detected between frames
                    isMoving = true;
                    break;
                }
                else
                {
                    isMoving = false;
                }
            }
        }
    }
}
